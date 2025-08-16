using System.Collections.Concurrent;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class KeyValueStore
    {
        /// <summary>
        /// Thread-safe dictionary storing simple key-value pairs.
        /// </summary>
        public static readonly ConcurrentDictionary<string, string> _kvStore = RamDb.KvStore;

        /// <summary>
        ///     Sets a key-value pair in the in-memory database. If the key already exists, its value is updated.
        /// </summary>
        /// <param name="key">The key to set or update in the database.</param>
        /// <param name="value">The value to associate with the specified key.</param>
        /// <returns>A confirmation string if the operation is successful, or null if the key is expired.</returns>
        public static string Set(string key, string value)
        {
            _kvStore[key] = value;
            return "OK";
        }

        /// <summary>
        ///     Retrieves the value associated with the specified key from the database if it exists and is not expired.
        /// </summary>
        /// <param name="key">The key whose associated value is to be retrieved.</param>
        /// <returns>The value associated with the specified key if it exists and is not expired, otherwise null.</returns>
        public static string Get(string key)
        {
            if (!_kvStore.ContainsKey(key)) return null;
            return _kvStore.GetValueOrDefault(key);
        }

        /// <summary>
        ///     Increments the value of a key by a specified amount.
        /// </summary>
        /// <param name="key">The key whose value is to be incremented.</param>
        /// <param name="increment">The amount to increment the key's value by.</param>
        /// <returns>The new value of the key after the increment, or 0 if the key is expired.</returns>
        public static long IncrBy(string key, long increment)
        {
            return _kvStore.AddOrUpdate(key,
                k => increment.ToString(),
                (k, v) =>
                {
                    var current = long.TryParse(v, out var parsed) ? parsed : 0;
                    return (current + increment).ToString();
                }) is { } updated
                ? long.Parse(updated)
                : 0;
        }

        /// <summary>
        ///     Increments the numeric value stored at the specified key by the given floating-point increment.
        ///     If the key does not exist, it is set to the increment value.
        /// </summary>
        /// <param name="key">The key of the value to increment.</param>
        /// <param name="increment">The floating-point value to add to the value stored at the key.</param>
        /// <returns>The updated value after the increment operation, or 0.0 if the key is expired, represented as a string.</returns>
        public static string IncrByFloat(string key, double increment)
        {
            return (_kvStore.AddOrUpdate(key,
                k => increment.ToString(),
                (k, v) =>
                {
                    var current = double.TryParse(v, out var parsed) ? parsed : 0.0;
                    return (current + increment).ToString();
                }) is { } updated
                ? double.Parse(updated)
                : 0.0).ToString();
        }
    }
}