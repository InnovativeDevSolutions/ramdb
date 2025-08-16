using System.Collections.Concurrent;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class GenericStore
    {
        /// <summary>
        /// Thread-safe dictionary storing hash sets where each key maps to another dictionary of key-value pairs.
        /// </summary>
        public static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _hashStore = RamDb.HashStore;

        /// <summary>
        /// Thread-safe dictionary storing simple key-value pairs.
        /// </summary>
        public static readonly ConcurrentDictionary<string, string> _kvStore = RamDb.KvStore;

        /// <summary>
        /// Thread-safe dictionary storing linked lists of strings, implementing list-based data structures.
        /// </summary>
        public static readonly ConcurrentDictionary<string, LinkedList<string>> _listStore = RamDb.ListStore;

        /// <summary>
        ///     Determines the number of specified keys that exist in the database.
        /// </summary>
        /// <param name="keys">An array of keys to check for existence in the database.</param>
        /// <returns>The count of keys that are found in the database.</returns>
        public static int Exists(params string[] keys)
        {
            var count = 0;

            foreach (var key in keys)
            {
                if (_kvStore.ContainsKey(key)) count++;
                if (_hashStore.ContainsKey(key)) count++;
                if (_listStore.ContainsKey(key)) count++;
            }

            return count;
        }

        /// <summary>
        ///     Deletes the specified keys from the database.
        /// </summary>
        /// <param name="keys">An array of keys to be removed from the database.</param>
        /// <returns>The count of keys successfully removed from the database.</returns>
        public static int Del(params string[] keys)
        {
            var count = 0;

            foreach (var key in keys)
            {
                if (_kvStore.TryRemove(key, out _)) count++;
                if (_hashStore.TryRemove(key, out _)) count++;
                if (_listStore.TryRemove(key, out _)) count++;
            }

            return count;
        }
    }
}