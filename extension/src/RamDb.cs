using System.Collections.Concurrent;
using System.Globalization;
using System.IO.Compression;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBePrivate.Global

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{

    /// <summary>
    /// Represents a RAM-based database implementation with key-value, hash, and list storage capabilities.
    /// </summary>
    public class RamDb : IDisposable
    {
        /// <summary>
        /// The current version of the RamDb implementation.
        /// </summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// Thread-safe dictionary storing hash sets where each key maps to another dictionary of key-value pairs.
        /// </summary>
        public static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> HashStore = new();

        /// <summary>
        /// Thread-safe dictionary storing simple key-value pairs.
        /// </summary>
        public static readonly ConcurrentDictionary<string, string> KvStore = new();

        /// <summary>
        /// Thread-safe dictionary storing linked lists of strings, implementing list-based data structures.
        /// </summary>
        public static readonly ConcurrentDictionary<string, LinkedList<string>> ListStore = new();

        /// <summary>
        /// Timer used for scheduling periodic database backups.
        /// </summary>
        private static Timer _backupTimer;

        #region Load/Save

        /// <summary>
        ///     Saves the current state of the database to a file, optionally creating a backup.
        /// </summary>
        /// <param name="createBackup">
        ///     A boolean value indicating whether a backup of the saved file should be created.
        ///     If true, a backup is created; otherwise, only the main file is saved.
        /// </param>
        public static bool SaveToFile(bool createBackup = false)
        {
            try
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, Main.RdbPath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                using (var writer = new BinaryWriter(gzip))
                {
                    Utils.WriteToDisc(writer);
                }

                if (!createBackup)
                {
                    Main.Log("RDB export complete", "action");
                    return true;
                }

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var backupDir = Path.Combine(Environment.CurrentDirectory, Main.RdbBackupFolder);

                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                var backupFilePath = Path.Combine(backupDir, $"data_{timestamp}.rdb.gz");

                using (var stream = new FileStream(backupFilePath, FileMode.Create))
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                using (var writer = new BinaryWriter(gzip))
                {
                    Utils.WriteToDisc(writer);
                }

                Main.Log($"RDB export complete (backup: {backupFilePath})", "action");
                return true;
            }
            catch (Exception e)
            {
                Main.Log($"Error while saving RDB: {e.Message}", "error");
                return false;
            }
        }

        /// <summary>
        ///     Loads data from a specified file into the database.
        /// </summary>
        /// <param name="filePath">The relative file path of the compressed database file to be loaded.</param>
        /// <returns>
        ///     A boolean value indicating whether the operation was successful. Returns false if the file does not exist or
        ///     an error occurs during loading.
        /// </returns>
        public static bool LoadFromFile(string filePath)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, filePath);
                if (!File.Exists(path))
                {
                    Main.Log($"RDB file not found: {path}", "error");
                    return false;
                }

                using var stream = new FileStream(path, FileMode.Open);
                using var gzip = new GZipStream(stream, CompressionMode.Decompress);
                using var reader = new BinaryReader(gzip);

                var fileVersion = reader.ReadInt32();
                if (fileVersion != CurrentVersion)
                {
                    Main.Log($"Unsupported RDB format version in backup: {fileVersion}", "warning");
                    return false;
                }

                KvStore.Clear();
                HashStore.Clear();
                ListStore.Clear();

                Utils.ReadFromDisc(reader);

                Main.Log($"RDB import complete (from: {path})", "info");
                return true;
            }
            catch (Exception e)
            {
                Main.Log($"Error while loading RDB: {e.Message}", "error");
                return false;
            }
        }

        /// <summary>
        ///     Retrieves a list of backup file names stored in the designated backup directory, ordered by their last writing time
        ///     in descending order.
        /// </summary>
        /// <returns>
        ///     A list of strings representing the names of backup files. Returns an empty list if the backup directory does
        ///     not exist or contains no files.
        /// </returns>
        public static List<string> GetBackupFiles()
        {
            var backupDir = Path.Combine(Environment.CurrentDirectory, Main.RdbBackupFolder);
            if (!Directory.Exists(backupDir)) return [];

            var files = Directory.GetFiles(backupDir, "data_*.rdb.gz").OrderByDescending(File.GetLastWriteTime);
            return [.. files.Select(Path.GetFileName)];
        }

        /// <summary>
        ///     Initializes the automatic backup system for the database based on predefined settings.
        /// </summary>
        /// <remarks>
        ///     This method sets up a periodic backup mechanism using a timer, triggered at intervals defined
        ///     in the configuration. If the automatic backup is disabled, the method exits without performing any operations.
        /// </remarks>
        public static void InitializeAutoBackup()
        {
            if (!Main.RdbAutoBackup) return;

            _backupTimer?.Dispose();
            _backupTimer = new Timer(BackupTimerCallback, null, TimeSpan.FromMinutes(Main.RdbBackupFrequency), TimeSpan.FromMinutes(Main.RdbBackupFrequency));

            Main.Log($"Automatic backup initialized (every {Main.RdbBackupFrequency} minutes)", "info");
        }

        /// <summary>
        ///     Callback method that handles the automatic backup process for the database.
        /// </summary>
        /// <param name="state">An optional state object passed to the callback.</param>
        private static void BackupTimerCallback(object state)
        {
            try
            {
                SaveToFile(true);
                ManageBackups();
                Main.Log($"Automatic backup created at {DateTime.UtcNow}", "info");
            }
            catch (Exception e)
            {
                Main.Log($"Error while backing up RDB: {e.Message}", "error");
            }
        }

        /// <summary>
        ///     Manages the backup files by ensuring the total number of backups does not exceed the configured limit.
        ///     Deletes excess backup files beyond the specified limit, keeping only the most recent backups.
        /// </summary>
        private static void ManageBackups()
        {
            try
            {
                var files = GetBackupFiles();
                if (files.Count <= Main.RdbMaxBackups) return;

                var backupDir = Path.Combine(Environment.CurrentDirectory, Main.RdbBackupFolder);
                var toDelete = files.Skip(Main.RdbMaxBackups).ToList();
                foreach (var file in toDelete)
                {
                    if (file != null)
                    {
                        File.Delete(Path.Combine(backupDir, file));
                        Main.Log($"Deleted backup file: {file}", "info");
                    }
                }
            }
            catch (Exception e)
            {
                Main.Log($"Error while managing backups: {e.Message}", "error");
            }
        }

        #endregion

        #region Disposable

        /// <summary>
        ///     Releases all resources used by the RamDb instance and performs cleanup operations.
        /// </summary>
        /// <remarks>
        ///     This method saves the current state of the database to a backup file, disposes of
        ///     the backup timer, logs the disposal operation, and suppresses finalization to optimize
        ///     garbage collection. If an exception occurs during the disposal process, it is logged.
        /// </remarks>
        public void Dispose()
        {
            try
            {
                SaveToFile(true);
                _backupTimer?.Dispose();
                Main.Log("RDB disposed", "info");
            }
            catch (Exception e)
            {
                Main.Log($"Error while disposing RDB: {e.Message}", "error");
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}