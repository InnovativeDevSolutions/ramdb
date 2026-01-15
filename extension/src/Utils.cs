using System.Collections.Concurrent;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    ///     Provides utility methods for common operations such as Base64 encoding/decoding, 
    ///     unique ID generation, and string manipulation.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        ///     Decodes a Base64 encoded string into its original plain text representation.
        ///     This method converts the input Base64 encoded string into a byte array
        ///     and then decodes the byte array into a plain text string using UTF-8 encoding.
        /// </summary>
        /// <param name="base64EncodedData">The Base64 encoded string to be decoded.</param>
        /// <return>A string containing the original plain text representation of the input Base64 data.</return>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        ///     Encodes a plain text string into its Base64 representation.
        ///     This method converts the input plain text into a byte array using UTF-8 encoding
        ///     and then encodes the byte array into a Base64 string.
        /// </summary>
        /// <param name="plainText">The plain text string to be encoded.</param>
        /// <return>A string containing the Base64 encoded representation of the input text.</return>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        ///     Generates a unique identifier based on the current UTC timestamp.
        ///     This method creates a unique identifier by retrieving the number of
        ///     milliseconds since the Unix epoch (January 1, 1970, 00:00:00 UTC).
        /// </summary>
        /// <return>A long value representing the unique identifier generated from the current UTC timestamp.</return>
        public static long GenerateUniqueId()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        ///     Splits a given string into smaller chunks of specified size.
        ///     This method divides the input string into multiple substrings based
        ///     on the specified chunk size, ensuring none exceed the given limit.
        /// </summary>
        /// <param name="data">The string to be divided into smaller chunks.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <return>A list of substrings, each with a length up to the specified chunk size.</return>
        public static List<string> SplitIntoChunks(string data, int chunkSize)
        {
            var totalChunks = (int)Math.Ceiling(data.Length / (double)chunkSize);
            List<string> chunks = [];

            for (var i = 0; i < totalChunks; i++)
            {
                var start = i * chunkSize;
                var end = Math.Min(data.Length, start + chunkSize);
                chunks.Add(data[start..end]);
            }

            return chunks;
        }

        /// <summary>
        ///     Writes the in-memory key-value, hash, and list data structures
        ///     to a binary stream for persistence. This method serializes the
        ///     application's state for storage in a file or other backup medium.
        /// </summary>
        /// <param name="writer">The BinaryWriter instance to write the serialized data to.</param>
        public static void WriteToDisc(BinaryWriter writer)
        {
            // Create snapshots of all collections to avoid concurrent modification issues
            var kvSnapshot = RamDb.KvStore.ToArray();
            var hashSnapshot = RamDb.HashStore.ToArray();
            var listSnapshot = RamDb.ListStore.ToArray();

            writer.Write(RamDb.CurrentVersion);
            
            // Write KV Store
            writer.Write(kvSnapshot.Length);
            foreach (var pair in kvSnapshot)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }

            // Write Hash Store
            writer.Write(hashSnapshot.Length);
            foreach (var hash in hashSnapshot)
            {
                var hashFieldsSnapshot = hash.Value.ToArray();
                writer.Write(hash.Key);
                writer.Write(hashFieldsSnapshot.Length);
                foreach (var field in hashFieldsSnapshot)
                {
                    writer.Write(field.Key);
                    writer.Write(field.Value);
                }
            }

            // Write List Store
            writer.Write(listSnapshot.Length);
            foreach (var list in listSnapshot)
            {
                var listItemsSnapshot = list.Value.ToArray();
                writer.Write(list.Key);
                writer.Write(listItemsSnapshot.Length);
                foreach (var item in listItemsSnapshot)
                {
                    writer.Write(item);
                }
            }
        }

        /// <summary>
        ///     Reads data from a binary stream and populates the in-memory key-value, hash,
        ///     and list data structures with the deserialized content. This method is
        ///     typically used when restoring the application's state from a backup file.
        /// </summary>
        /// <param name="reader">The BinaryReader instance to read the serialized data from.</param>
        public static void ReadFromDisc(BinaryReader reader)
        {
            var kvCount = reader.ReadInt32();
            for (var i = 0; i < kvCount; i++)
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                RamDb.KvStore.TryAdd(key, value);
                Main.Log($"Loaded key-value: {key} = {value[..Math.Min(50, value.Length)]}...", "debug");
            }

            var hashCount = reader.ReadInt32();
            for (var i = 0; i < hashCount; i++)
            {
                var key = reader.ReadString();
                var fieldCount = reader.ReadInt32();
                Main.Log($"Loading hash: {key}", "debug");

                var hash = new ConcurrentDictionary<string, string>();
                for (var j = 0; j < fieldCount; j++)
                {
                    var field = reader.ReadString();
                    var value = reader.ReadString();
                    hash.TryAdd(field, value);
                    Main.Log($"Loaded field: {field} = {value[..Math.Min(50, value.Length)]}...", "debug");
                }

                RamDb.HashStore.TryAdd(key, hash);
            }

            var listCount = reader.ReadInt32();
            for (var i = 0; i < listCount; i++)
            {
                var key = reader.ReadString();
                var itemCount = reader.ReadInt32();
                Main.Log($"Loading list: {key}", "debug");

                var list = new LinkedList<string>();
                for (var j = 0; j < itemCount; j++)
                {
                    var item = reader.ReadString();
                    list.AddLast(item);
                    Main.Log($"Loaded item: {item[..Math.Min(50, item.Length)]}...", "debug");
                }

                RamDb.ListStore.TryAdd(key, list);
            }

            Main.Log("Loaded data from disk", "debug");
        }

        /// <summary>
        ///     Processes a given data string, checks its byte size, and splits it into chunks if it exceeds the specified buffer
        ///     size.
        ///     If chunking is necessary, it sends each chunk to a callback function and returns metadata about the operation.
        /// </summary>
        /// <param name="uniqueId">A unique identifier used to associate the processed data.</param>
        /// <param name="bufferSize">The maximum permissible byte size for the data before it is split into chunks.</param>
        /// <param name="data">The data string to be processed, analyzed for byte size, and optionally chunked.</param>
        /// <param name="function">An optional function name associated with the processing, or null if not applicable.</param>
        /// <param name="entity">An optional entity name linked to the processing operation, or null if it does not apply.</param>
        /// <param name="call">A boolean value indicating whether to invoke a callback function during processing.</param>
        /// <return>
        ///     A string representing either the processed data or metadata, including chunking status and the unique
        ///     identifier.
        /// </return>
        public static string CheckByteCount(string uniqueId, int bufferSize, string data, string function = "",
            string entity = "", bool call = false)
        {
            var byteCount = Encoding.UTF8.GetByteCount(data);

            if (!data.StartsWith('[') || !data.EndsWith(']')) data = Main.SerializeList([.. data.Split(',')]);
            if (byteCount > bufferSize)
            {
                var chunks = SplitIntoChunks(data, bufferSize);
                var totalChunks = chunks.Count;

                for (var i = 0; i < totalChunks; i++)
                {
                    var escapedChunk = chunks[i].Replace("\"", "\"\"");
                    var chunk = $"[\"{uniqueId}\", \"{function}\", {i + 1}, {totalChunks}, \"{escapedChunk}\", {call.ToString().ToLower()}, \"{entity}\"]";
                    Main.Log($"Chunk data: {chunk}", "debug");
                    Main.Callback("ArmaRAMDb", "ramdb_db_fnc_fetch", chunk);
                }

                return "OK";
            }

            Main.Log($"Data: {data}", "debug");
            return data;
        }
    }
}