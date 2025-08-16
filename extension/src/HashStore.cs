using System.Collections.Concurrent;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class HashStore
    {
        /// <summary>
        /// Thread-safe dictionary storing hash sets where each key maps to another dictionary of key-value pairs.
        /// </summary>
        public static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _hashStore = RamDb.HashStore;

        /// <summary>
        ///     Sets the specified field in the hash stored at the given key to the provided value.
        /// </summary>
        /// <param name="key">The key identifying the hash in the database.</param>
        /// <param name="field">The field within the hash to set or update.</param>
        /// <param name="value">The value to assign to the specified field.</param>
        /// <returns>1 if a new field was added, or 0 if an existing field was updated.</returns>
        public static int HSet(string key, string field, string value)
        {
            var hash = _hashStore.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());
            if (hash.TryAdd(field, value)) return 1;

            hash[field] = value;
            return 0;
        }

        /// <summary>
        ///     Sets multiple field-value pairs in a hash stored at the specified key.
        /// </summary>
        /// <param name="key">The key associated with the hash in the database.</param>
        /// <param name="fieldValues">A dictionary containing field-value pairs to be set in the hash.</param>
        /// <returns>The number of new fields added to the hash. Existing fields are overwritten and not counted.</returns>
        public static int HMSet(string key, Dictionary<string, string> fieldValues)
        {
            var hash = _hashStore.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());
            var count = 0;
            foreach (var pair in fieldValues)
                if (hash.TryAdd(pair.Key, pair.Value)) count++;
                else hash[pair.Key] = pair.Value;
            return count;
        }

        /// <summary>
        ///     Retrieves the value associated with the specified field in the hash stored at the given key.
        /// </summary>
        /// <param name="key">The key of the hash from which the value is to be retrieved.</param>
        /// <param name="field">The field within the hash whose value needs to be retrieved.</param>
        /// <returns>
        ///     The value associated with the specified field, or null if the key does not exist, the field is not present in
        ///     the hash, or the key has expired.
        /// </returns>
        public static string HGet(string key, string field)
        {
            return _hashStore.TryGetValue(key, out var hash) && hash.TryGetValue(field, out var value)
                ? value
                : null;
        }

        /// <summary>
        ///     Retrieves all the fields and values stored in a hash associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the hash to retrieve.</param>
        /// <returns>
        ///     A dictionary containing all fields and their corresponding values from the hash. If the key does not exist or
        ///     is expired, it returns an empty dictionary.
        /// </returns>
        public static Dictionary<string, string> HGetAll(string key)
        {
            return _hashStore.TryGetValue(key, out var hash) ? new Dictionary<string, string>(hash) : [];
        }

        /// <summary>
        ///     Removes the specified fields from the hash stored at the given key.
        /// </summary>
        /// <param name="key">The key identifying the hash in the database.</param>
        /// <param name="fields">An array of field names to be removed from the hash.</param>
        /// <returns>The number of fields that were successfully removed from the hash.</returns>
        public static int HDel(string key, params string[] fields)
        {
            return !_hashStore.TryGetValue(key, out var hash) ? 0 : fields.Count(field => hash.TryRemove(field, out _));
        }

        /// <summary>
        ///     Determines whether the specified field exists in the hash stored at the given key.
        /// </summary>
        /// <param name="key">The key of the hash to check.</param>
        /// <param name="field">The field within the hash to check for existence.</param>
        /// <returns>1 if the field exists in the hash, or 0 if it does not.</returns>
        public static int HExists(string key, string field)
        {
            return _hashStore.TryGetValue(key, out var hash) && hash.ContainsKey(field) ? 1 : 0;
        }

        /// <summary>
        ///     Retrieves the number of fields contained in the hash stored at the specified key.
        /// </summary>
        /// <param name="key">The key of the hash whose field count is to be determined.</param>
        /// <returns>The number of fields in the hash if the key exists; otherwise, 0.</returns>
        public static int HLen(string key)
        {
            return _hashStore.TryGetValue(key, out var hash) ? hash.Count : 0;
        }

        /// <summary>
        ///     Retrieves all field names of a hash stored at the specified key in the database.
        /// </summary>
        /// <param name="key">The key of the hash from which to retrieve the field names.</param>
        /// <returns>A list of field names stored in the hash, or an empty list if the key does not exist or is expired.</returns>
        public static List<string> HKeys(string key)
        {
            return _hashStore.TryGetValue(key, out var hash) ? [.. hash.Keys] : [];
        }

        /// <summary>
        ///     Retrieves all values associated with a hash key in the database.
        /// </summary>
        /// <param name="key">The hash key for which to retrieve all values.</param>
        /// <returns>
        ///     A list of values associated with the specified hash key.
        ///     Returns an empty list if the key does not exist or is expired.
        /// </returns>
        public static List<string> HVals(string key)
        {
            return _hashStore.TryGetValue(key, out var hash) ? [.. hash.Values] : [];
        }

        /// <summary>
        ///     Increments the integer value of a hash field by a specified increment. If the field does not exist, it is created
        ///     and set to the increment value.
        /// </summary>
        /// <param name="key">The key of the hash that contains the field to increment.</param>
        /// <param name="field">The name of the field whose value will be increased.</param>
        /// <param name="increment">The amount by which to increment the field's value.</param>
        /// <returns>The new value of the field after the increment operation.</returns>
        public static long HIncrBy(string key, string field, long increment)
        {
            var hash = _hashStore.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());

            return hash.AddOrUpdate(field,
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
        ///     Increments the value of a field in a hash by a specified floating-point increment.
        ///     If the field does not exist, it is initialized to 0 before the increment is applied.
        /// </summary>
        /// <param name="key">The key identifying the hash.</param>
        /// <param name="field">The field within the hash to increment.</param>
        /// <param name="increment">The floating-point value by which to increment the field.</param>
        /// <returns>The new value of the field after the increment is applied.</returns>
        public static string HIncrByFloat(string key, string field, double increment)
        {
            var hash = _hashStore.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());

            return (hash.AddOrUpdate(field,
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
