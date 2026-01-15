using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable PropertyCanBeMadeInitOnly.Global

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{

    /// <summary>
    /// Represents the context information for a remote procedure call.
    /// </summary>
    public class CallContext
    {
        /// <summary>
        /// Gets or sets the Steam ID of the user making the call.
        /// </summary>
        public ulong SteamId { get; set; }

        /// <summary>
        /// Gets or sets the source file path of the call.
        /// </summary>
        public string FileSource { get; set; }

        /// <summary>
        /// Gets or sets the name of the mission associated with the call.
        /// </summary>
        public string MissionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the server where the call originated.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the owner who remotely executed the call.
        /// </summary>
        public short RemoteExecutedOwner { get; set; }
    }

    /// <summary>
    /// Main class containing core functionality and configuration settings.
    /// </summary>
    public static class Main
    {
        /// <summary>
        /// The current version of RamDb.
        /// </summary>
        private const string RdbVersion = "1.0.0";

        /// <summary>
        /// The buffer size for database operations.
        /// </summary>
        public const int RdbBufferSize = 20480;

        /// <summary>
        /// The main database instance.
        /// </summary>
        private static RamDb _db;

        /// <summary>
        /// Gets the main database instance.
        /// </summary>
        public static RamDb Db => _db;

        /// <summary>
        /// Gets the path for main database file.
        /// </summary>
        public static readonly string RdbPath = $"@ramdb{Path.DirectorySeparatorChar}data.rdb.gz";

        /// <summary>
        /// Gets the path for config file.
        /// </summary>
        public static readonly string RdbConfigPath = $"{Path.DirectorySeparatorChar}@ramdb{Path.DirectorySeparatorChar}config.xml";

        /// <summary>
        /// Gets the folder path for database backups.
        /// </summary>
        public static readonly string RdbBackupFolder = $"@ramdb{Path.DirectorySeparatorChar}backups";

        /// <summary>
        /// Gets the folder path for storing log files.
        /// </summary>
        public static readonly string RdbLogFolder = $"{Path.DirectorySeparatorChar}@ramdb{Path.DirectorySeparatorChar}logs";

        /// <summary>
        /// Gets or sets whether debug mode is enabled.
        /// </summary>
        public static bool RdbDebug { get; private set; } = false;

        /// <summary>
        /// Gets or sets whether an initialization check has been performed.
        /// </summary>
        public static bool RdbInitCheck { get; private set; } = false;

        /// <summary>
        /// Gets or sets whether automatic backup is enabled.
        /// </summary>
        public static bool RdbAutoBackup { get; private set; } = false;

        /// <summary>
        /// Gets or sets the frequency of automatic backups in minutes.
        /// </summary>
        public static int RdbBackupFrequency { get; private set; } = 0;

        /// <summary>
        /// Gets or sets the maximum number of backups to maintain.
        /// </summary>
        public static int RdbMaxBackups { get; private set; } = 0;

        /// <summary>
        /// Gets or sets the Steam ID of the current user.
        /// </summary>
        public static string SteamId { get; private set; } = "";

        /// <summary>
        /// Gets or sets whether context logging is enabled.
        /// </summary>
        public static bool RdbContextLog { get; private set; } = false;

        /// <summary>
        /// Gets or sets the pointer to the current RamDb context.
        /// </summary>
        public static IntPtr RdbContext { get; private set; }

        /// <summary>
        /// Gets or sets whether the database is currently loaded.
        /// </summary>
        public static bool RdbIsLoaded { get; private set; } = false;

        /// <summary>
        /// Delegate for extension callback functions that process extension operations.
        /// </summary>
        /// <param name="name">The name of the extension.</param>
        /// <param name="function">The function to be called.</param>
        /// <param name="data">The data to be processed.</param>
        /// <returns>Integer result code of the operation.</returns>
        public delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);

        /// <summary>
        /// Gets or sets the callback function for extension operations.
        /// </summary>
        public static ExtensionCallback Callback { get; private set; }

        /// <summary>
        /// Resolves a player key to a valid Steam ID format. This method handles several cases:
        /// 1. If the key is null, empty, or contains only quotes, returns the current Steam ID
        /// 2. If the key exactly matches "_SP_PLAYER_", returns the current Steam ID
        /// 3. If the key starts with "_SP_PLAYER_:", replaces that prefix with the current Steam ID
        /// 4. Otherwise, returns the key with any surrounding quotes removed
        /// </summary>
        /// <param name="key">The player key to resolve. Can be null, empty, or contain special placeholders.</param>
        /// <returns>A string representing the resolved player key, either the current Steam ID or a processed version of the input key.</returns>
        private static string ResolveKey(string key)
        {
            if (string.IsNullOrEmpty(key) || key == "" || key == "\"\"")
                return SteamId;

            var finalKey = key.Trim('"');

            if (finalKey == "_SP_PLAYER_")
                return SteamId;

            if (finalKey.StartsWith("_SP_PLAYER_:"))
                return finalKey.Replace("_SP_PLAYER_", SteamId);

            return finalKey;
        }

        /// <summary>
        ///     Initializes the application by reading the configuration from a specified
        ///     file or using default settings if the file is not found. It sets up the
        ///     main database, logging, debugging, and other runtime parameters.
        /// </summary>
        private static void Init()
        {
            char[] separator = ['='];
            var commandLineArgs = Environment.GetCommandLineArgs();
            var str = "";

            for (int i = 0; i < commandLineArgs.Length; i++)
            {
                var strArray = commandLineArgs[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (strArray[0].Equals("-rdb", StringComparison.CurrentCultureIgnoreCase))
                {
                    str = strArray[1];
                    break;
                }
            }

            if (!File.Exists(Environment.CurrentDirectory + RdbConfigPath))
                Log("Config file not found! Default settings will be used.", "action");

            try
            {
                var configXml = XElement.Load(Environment.CurrentDirectory + RdbConfigPath);
                List<string> settings = [.. configXml.Elements().Select(element => (string)element)];

                if (bool.TryParse(settings[0], out var res0))
                    RdbContextLog = res0;
                if (bool.TryParse(settings[1], out var res1))
                    RdbDebug = res1;
                if (bool.TryParse(settings[2], out var res2))
                    RdbAutoBackup = res2;
                if (int.TryParse(settings[3], out var res3) && res3 > 0)
                    RdbBackupFrequency = res3;
                if (int.TryParse(settings[4], out var res4) && res4 > 0)
                    RdbMaxBackups = res4;

                Log($"Config file found! Context Mode: {RdbContextLog}! Debug Mode: {RdbDebug}! Auto Backup: {RdbAutoBackup} (every {RdbBackupFrequency} min, keep {RdbMaxBackups})", "action");

                _db = new RamDb();

                if (File.Exists(Path.Combine(Environment.CurrentDirectory, RdbPath)))
                {
                    RamDb.LoadFromFile(RdbPath);
                    RdbIsLoaded = true;

                    Log("Existing RDB data loaded during initialization", "action");
                }

                if (RdbAutoBackup)
                {
                    RamDb.InitializeAutoBackup();
                }
            }
            catch (Exception e)
            {
                Log($"Error reading config file: {e.Message}", "error");
                Log("Default settings will be used", "action");
            }

            RdbInitCheck = true;
        }

        /// <summary>
        ///     Logs a message to a file of the specified type if debugging is enabled.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">The type of log (e.g., "error", "action").</param>
        public static void Log(string message, string type)
        {
            if (!RdbDebug)
                return;

            var fileName = $"{type}.log";
            var filePath = Environment.CurrentDirectory + RdbLogFolder;

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            using StreamWriter sw = new(Path.Combine(filePath, fileName), true);
            sw.WriteLine($"{DateTime.Now} >> {message}");
        }

        /// <summary>
        ///     Called only once when Arma 3 loads the extension.
        /// </summary>
        /// <param name="func">Pointer to Arma 3's callback function</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionRegisterCallback")]
        public unsafe static void RVExtensionRegisterCallback(nint func)
        {
            Callback = Marshal.GetDelegateForFunctionPointer<ExtensionCallback>(func);
        }

        /// <summary>
        ///     Called only once when Arma 3 loads the extension.
        ///     The output will be written in the RPT logs.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="outputSize">The maximum length of the buffer (always 32 for this particular method)</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionVersion")]
        public unsafe static void RVExtensionVersion(char* output, int outputSize)
        {
            WriteOutput(output, $"RAMDb {RdbVersion}");
        }

        /// <summary>
        ///     Called before every RVExtension/RVExtensionArgs execution, providing context about where the call came from.
        /// </summary>
        /// <param name="argv">
        ///     Array of pointers to null-terminated strings containing:
        ///     [0] - Steam 64bit UserID
        ///     [1] - FileSource
        ///     [2] - MissionName
        ///     [3] - ServerName
        ///     [4] - Machine Network ID
        /// </param>
        /// <param name="argc">Number of arguments in the argv array (always 5).</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionContext")]
        public unsafe static int RvExtensionContext(char** argv, int argc)
        {
            RdbContext = (IntPtr)argv;

            var context = new CallContext
            {
                SteamId = ulong.Parse(Marshal.PtrToStringAnsi((IntPtr)argv[0]) ?? "0"),
                FileSource = Marshal.PtrToStringAnsi((IntPtr)argv[1]) ?? "",
                MissionName = Marshal.PtrToStringAnsi((IntPtr)argv[2]) ?? "",
                ServerName = Marshal.PtrToStringAnsi((IntPtr)argv[3]) ?? "",
                RemoteExecutedOwner = short.Parse(Marshal.PtrToStringAnsi((IntPtr)argv[4]) ?? "0")
            };

            SteamId = context.SteamId.ToString();

            if (RdbContextLog)
            {
                Log(
                    $"Context: SteamID={context.SteamId}, FileSource={context.FileSource}, MissionName={context.MissionName}, ServerName={context.ServerName}, RemoteExecutedOwner={context.RemoteExecutedOwner}",
                    "context");
            }

            return 100;
        }

        /// <summary>
        ///     The entry point for the default "callExtension" command.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="outputSize">The maximum size of the buffer (20,480 bytes)</param>
        /// <param name="function">The function identifier passed in "callExtension"</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtension")]
        public unsafe static void RVExtension(char* output, int outputSize, char* function)
        {
            if (!RdbInitCheck)
                Init();

            _db ??= new RamDb();

            var func = GetString(function);
            switch (func.ToLower())
            {
                case "time":
                    WriteOutput(output, DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss"));
                    break;
                case "version":
                    WriteOutput(output, RdbVersion);
                    break;
                default:
                    WriteOutput(output, func);
                    break;
            }
        }

        /// <summary>
        ///     The entry point for the "callExtension" command with function arguments.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="outputSize">The maximum size of the buffer (20,480 bytes)</param>
        /// <param name="function">The function identifier passed in "callExtension"</param>
        /// <param name="argv">The args passed to "callExtension" as a string array</param>
        /// <param name="argc">Number of elements in "argv"</param>
        /// <returns>The return code</returns>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionArgs")]
        public unsafe static int RVExtensionArgs(char* output, int outputSize, char* function, char** argv, int argc)
        {
            if (!RdbInitCheck)
                Init();

            _db ??= new RamDb();

            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var func = GetString(function);

            List<string> args = [];
            for (int i = 0; i < argc; i++)
            {
                args.Add(GetString(argv[i]));
            }

            var count = 0;

            Log($"Function: {func}, Args: {string.Join(", ", args)}", "debug");

            switch (func.ToLower())
            {
                case "set":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var set = KeyValueStore.Set(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, $"{set}");
                    return 100;
                case "get":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var get = KeyValueStore.Get(ResolveKey(args[0]));

                    WriteOutput(output, $"{get}");
                    return 200;
                case "del":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    foreach (var arg in args)
                    {
                        count += GenericStore.Del(arg.Trim('"'));
                    }

                    WriteOutput(output, $"{count}");
                    return 100;
                case "exists":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    foreach (var arg in args)
                    {
                        count += GenericStore.Exists(arg.Trim('"'));
                    }

                    WriteOutput(output, $"{count}");
                    return 200;
                case "hset":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    HashStore.HSet(ResolveKey(args[0]), args[1], args[2]);

                    WriteOutput(output, "OK");
                    return 100;
                case "hmset":
                    if (argc < 2 || (argc - 1) % 2 != 0)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var fieldValues = new Dictionary<string, string>();

                    for (var i = 1; i < argc; i += 2) fieldValues.Add(args[i], args[i + 1]);

                    HashStore.HMSet(ResolveKey(args[0]), fieldValues);

                    WriteOutput(output, "OK");
                    return 100;
                case "hget":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hget = HashStore.HGet(ResolveKey(args[0]), args[1]);

                    if (hget is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hget", RdbBufferSize, hget,
                            args[2], args[3], Convert.ToBoolean(args[4])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hget", RdbBufferSize, hget));
                    }

                    return 200;
                case "hgetall":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hgetall = HashStore.HGetAll(ResolveKey(args[0]));

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hgetall", RdbBufferSize, SerializeList([.. hgetall.SelectMany(kvp => new[] { kvp.Key, kvp.Value })]),
                            args[1], args[2], Convert.ToBoolean(args[3])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hgetall", RdbBufferSize, SerializeList([.. hgetall.SelectMany(kvp => new[] { kvp.Key, kvp.Value })])));
                    }

                    return 200;
                case "hdel":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var fieldsToDelete = new List<string>();

                    for (var i = 1; i < argc; i++) fieldsToDelete.Add(args[i]);

                    count = HashStore.HDel(ResolveKey(args[0]), [.. fieldsToDelete]);

                    WriteOutput(output, $"{count}");
                    return 200;
                case "hlen":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hlen = HashStore.HLen(ResolveKey(args[0]));

                    if (hlen == 0)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    WriteOutput(output, $"{hlen}");
                    return 200;
                case "hkeys":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hkeys = HashStore.HKeys(ResolveKey(args[0]));

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hkeys", RdbBufferSize, SerializeList(hkeys),
                            args[1], args[2], Convert.ToBoolean(args[3])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hkeys", RdbBufferSize, SerializeList(hkeys)));
                    }

                    return 200;
                case "hvals":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hvals = HashStore.HVals(ResolveKey(args[0]));

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hvals", RdbBufferSize, SerializeList(hvals),
                            args[1], args[2], Convert.ToBoolean(args[3])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hvals", RdbBufferSize, SerializeList(hvals)));
                    }

                    return 200;
                case "hexists":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hexists = HashStore.HExists(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, $"{hexists}");
                    return 200;
                case "hincrby":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hincrby = HashStore.HIncrBy(ResolveKey(args[0]), args[1], Convert.ToInt32(args[2].Trim('"')));

                    WriteOutput(output, $"{hincrby}");
                    return 200;
                case "hincrbyfloat":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hincrbyfloat = HashStore.HIncrByFloat(ResolveKey(args[0]), args[1], Convert.ToDouble(args[2].Trim('"')));

                    WriteOutput(output, hincrbyfloat);
                    return 200;
                case "lindex":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var listIndex = ListStore.LIndex(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')));

                    if (listIndex is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    if (argc >= 5)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_lindex", RdbBufferSize, listIndex,
                            args[2], args[3], Convert.ToBoolean(args[4])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_lindex", RdbBufferSize, listIndex));
                    }

                    return 200;
                case "llen":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var listLength = ListStore.LLen(ResolveKey(args[0]));

                    WriteOutput(output, $"{listLength}");
                    return 200;
                case "linsert":
                    if (argc < 4)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var listInsert = ListStore.LInsert(ResolveKey(args[0]), Convert.ToBoolean(args[1].Trim('"')), args[2], args[3]);

                    WriteOutput(output, $"{listInsert}");
                    return 200;
                case "lpop":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lpop = ListStore.LPop(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')));

                    if (lpop is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    WriteOutput(output, SerializeList(lpop));
                    return 200;
                case "lpush":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lpush = ListStore.LPush(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, $"{lpush}");
                    return 200;
                case "lrange":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lrange = ListStore.LRange(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')), Convert.ToInt32(args[2].Trim('"')));

                    WriteOutput(output, SerializeList(lrange));
                    return 200;
                case "lrem":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lrem = ListStore.LRem(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')), args[2]);

                    WriteOutput(output, $"{lrem}");
                    return 200;
                case "lset":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lset = ListStore.LSet(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')), args[2]);

                    if (lset is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    WriteOutput(output, lset);
                    return 200;
                case "ltrim":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var ltrim = ListStore.LTrim(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')), Convert.ToInt32(args[2].Trim('"')));

                    if (ltrim is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    WriteOutput(output, ltrim);
                    return 200;
                case "rpop":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var rpop = ListStore.RPop(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')));

                    if (rpop is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    WriteOutput(output, SerializeList(rpop));
                    return 200;
                case "rpush":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var rpush = ListStore.RPush(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, $"{rpush}");
                    return 200;
                case "incrby":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var incrby = KeyValueStore.IncrBy(ResolveKey(args[0]), Convert.ToInt32(args[1].Trim('"')));

                    WriteOutput(output, $"{incrby}");
                    return 200;
                case "incrbyfloat":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var incrbyfloat = KeyValueStore.IncrByFloat(ResolveKey(args[0]), Convert.ToDouble(args[1].Trim('"')));

                    WriteOutput(output, incrbyfloat);
                    return 200;
                case "save":
                    WriteOutput(output, RamDb.SaveToFile(Convert.ToBoolean(args[0]))
                        ? "Saved data to disc"
                        : "Error saving to disc");
                    return 100;
                case "load":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    WriteOutput(output, RamDb.LoadFromFile(args[0].Trim('"'))
                        ? "Loaded data from disc"
                        : "Error loading from disc");
                    return 100;
                case "version":
                    WriteOutput(output, RdbVersion);
                    return 100;
                default:
                    WriteOutput(output, "Available functions: set, get, del, exists, hset, hmset, hget, hgetall, hdel, hlen, hkeys, hvals, hexists, hincrby, hincrbyfloat, lindex, llen, linsert, lpop, lpush, lrange, lrem, lset, ltrim, rpop, rpush, incrby, incrbyfloat, save, version");
                    return -1;
            }
        }

        /// <summary>
        ///     Reads a string from the given pointer.
        ///     If the pointer is null, a default value will be returned instead.
        /// </summary>
        /// <param name="pointer">The pointer to read</param>
        /// <param name="defaultValue">The string's default value</param>
        /// <returns>The marshaled string</returns>
        private unsafe static string GetString(char* pointer, string defaultValue = "")
        {
            return Marshal.PtrToStringAnsi((nint)pointer) ?? defaultValue;
        }

        /// <summary>
        ///     Serializes a list of strings into a string representing a valid Arma 3 array.
        /// </summary>
        /// <param name="list">The list of strings to serialize</param>
        /// <returns>A string representing an Arma 3 array</returns>
        public static string SerializeList(List<string> list)
        {
            var content = string.Join(",", [.. list]);
            return string.Format("[{0}]", content);
        }

        /// <summary>
        ///     Writes the given payload to the output buffer using the provided pointer.
        ///     Ensures that the payload string is always null terminated.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="payload">The payload to write</param>
        /// <param name="maxSize">Maximum size of the output buffer (default: RdbBufferSize)</param>
        private unsafe static void WriteOutput(char* output, string payload, int maxSize = RdbBufferSize)
        {
            Log($"Writing output: {payload}", "info");

            byte[] bytes = Encoding.ASCII.GetBytes(payload + '\0');
            
            // Prevent buffer overrun by truncating if necessary
            if (bytes.Length > maxSize)
            {
                Log($"Output truncated from {bytes.Length} to {maxSize} bytes", "warning");
                bytes = bytes.Take(maxSize - 1).Concat(new byte[] { 0 }).ToArray();
            }
            
            Marshal.Copy(bytes, 0, (nint)output, bytes.Length);
        }
    }
}