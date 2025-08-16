using System.Collections.Concurrent;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaRAMDb
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class ListStore
    {
        /// <summary>
        /// Thread-safe dictionary storing linked lists where each key maps to a linked list of strings.
        /// </summary>
        public static readonly ConcurrentDictionary<string, LinkedList<string>> _listStore = RamDb.ListStore;

        /// <summary>
        ///     Inserts the specified values at the head of the list stored at the given key. If the key does not exist, a new list
        ///     is created.
        /// </summary>
        /// <param name="key">The key identifying the list where the values will be inserted.</param>
        /// <param name="values">An array of values to be inserted at the head of the list.</param>
        /// <returns>The total number of elements in the list after the operation.</returns>
        public static int LPush(string key, params string[] values)
        {
            var list = _listStore.GetOrAdd(key, _ => new LinkedList<string>());
            foreach (var value in values)
            {
                list.AddFirst(value);
            }

            return list.Count;
        }

        /// <summary>
        ///     Appends one or more values to the end of a list stored at the specified key.
        /// </summary>
        /// <param name="key">The key of the list where the values will be appended.</param>
        /// <param name="values">An array of values to append to the list.</param>
        /// <returns>The total number of elements in the list after the operation</returns>
        public static int RPush(string key, params string[] values)
        {
            var list = _listStore.GetOrAdd(key, _ => new LinkedList<string>());
            foreach (var value in values) list.AddLast(value);

            return list.Count;
        }

        /// <summary>
        ///     Removes and returns the specified number of elements from the head of the list stored at the given key.
        /// </summary>
        /// <param name="key">The key of the list from which elements will be removed.</param>
        /// <param name="count">The number of elements to remove from the head of the list. Defaults to 1.</param>
        /// <returns>A list of removed elements, or null if the key does not exist or refers to a non-list value.</returns>
        public static List<string> LPop(string key, int count = 1)
        {
            if (!_listStore.TryGetValue(key, out var list) || list.First == null) return null;

            var result = new List<string>();
            for (var i = 0; i < count && list.First != null; i++)
            {
                result.Add(list.First.Value);
                list.RemoveFirst();
            }

            return result;
        }

        /// <summary>
        ///     Removes and returns the specified number of elements from the end of the list stored at the given key.
        /// </summary>
        /// <param name="key">The key of the list from which elements are to be removed.</param>
        /// <param name="count">The number of elements to remove from the end of the list. Defaults to 1.</param>
        /// <returns>A list of removed elements, or null if the key does not exist or the list is empty.</returns>
        public static List<string> RPop(string key, int count = 1)
        {
            if (!_listStore.TryGetValue(key, out var list) || list.Last == null) return null;

            var result = new List<string>();
            for (var i = 0; i < count && list.Last != null; i++)
            {
                result.Add(list.Last.Value);
                list.RemoveLast();
            }

            return result;
        }

        /// <summary>
        ///     Retrieves a range of elements from a list stored at a specified key in the database.
        /// </summary>
        /// <param name="key">The key associated with the list from which the range of elements will be retrieved.</param>
        /// <param name="start">The starting index (inclusive) of the range within the list.</param>
        /// <param name="end">
        ///     The ending index (inclusive) of the range within the list. Use -1 to indicate the last element of the
        ///     list.
        /// </param>
        /// <returns>
        ///     A list containing the elements in the specified range. Returns an empty list if the key does not exist or the
        ///     range is invalid.
        /// </returns>
        public static List<string> LRange(string key, int start, int end)
        {
            if (!_listStore.TryGetValue(key, out var list)) return [];

            var result = new List<string>();
            var current = list.First;
            var index = 0;

            if (end == -1)
            {
                end = list.Count - 1;
            }

            while (current != null && index <= end)
            {
                if (index >= start) result.Add(current.Value);

                current = current.Next;
                index++;
            }

            return result;
        }

        /// <summary>
        ///     Retrieves an element from a list at a specified index.
        /// </summary>
        /// <param name="key">The key associated with the list in the database.</param>
        /// <param name="index">The index of the element to retrieve. Negative values count at the end of the list.</param>
        /// <returns>
        ///     The value of the element at the specified index, or null if the key does not exist, the index is out of range,
        ///     or the list is expired.
        /// </returns>
        public static string LIndex(string key, int index)
        {
            if (!_listStore.TryGetValue(key, out var list)) return null;

            if (index < 0) index += list.Count;
            if (index < 0 || index >= list.Count) return null;

            var node = list.First;
            for (var i = 0; i < index; i++) node = node?.Next;

            return node?.Value;
        }

        /// <summary>
        ///     Retrieves the number of elements in a list at the specified key.
        /// </summary>
        /// <param name="key">The key associated with the list in the database.</param>
        /// <returns>The number of elements in the list, or 0 if the key does not exist or is not associated with a list.</returns>
        public static int LLen(string key)
        {
            return _listStore.TryGetValue(key, out var list) ? list.Count : 0;
        }

        /// <summary>
        ///     Inserts an element in the list stored at the specified key before or after the pivot element.
        /// </summary>
        /// <param name="key">The key of the list where the element will be inserted.</param>
        /// <param name="before">A boolean indicating whether the element should be inserted before the pivot element.</param>
        /// <param name="pivot">The existing element in the list near which the new element will be inserted.</param>
        /// <param name="value">The value to be inserted into the list.</param>
        /// <returns>The length of the list after the insertion. Returns -1 if the pivot element is not found.</returns>
        public static int LInsert(string key, bool before, string pivot, string value)
        {
            if (!_listStore.TryGetValue(key, out var list)) return 0;

            var node = list.First;
            while (node != null)
            {
                if (node.Value == pivot)
                {
                    if (before)
                        list.AddBefore(node, value);
                    else
                        list.AddAfter(node, value);

                    return list.Count;
                }

                node = node.Next;
            }

            return -1;
        }

        /// <summary>
        ///     Sets the element at the specified index in the list stored at the given key to the provided value.
        /// </summary>
        /// <param name="key">The key of the list in the database.</param>
        /// <param name="index">
        ///     The zero-based index of the element to replace. Use negative indices to count from the end of the
        ///     list.
        /// </param>
        /// <param name="value">The new value to set at the specified index.</param>
        /// <returns>
        ///     Returns "OK" if the operation is successful, or null if the key does not exist, the index is out of range, or
        ///     the key is expired.
        /// </returns>
        public static string LSet(string key, int index, string value)
        {
            if (!_listStore.TryGetValue(key, out var list)) return null;

            if (index < 0) index += list.Count;
            if (index < 0 || index >= list.Count) return null;

            var node = list.First;
            for (var i = 0; i < index; i++) node = node?.Next;

            if (node != null) node.Value = value;
            return "OK";
        }

        /// <summary>
        ///     Removes the specified number of occurrences of a value from a list stored in the database.
        /// </summary>
        /// <param name="key">The key of the list from which the value should be removed.</param>
        /// <param name="count">
        ///     The number of occurrences to remove:
        ///     - If the count is greater than 0, removes up-to-count occurrences starting from the head of the list.
        ///     - If the count is less than 0, removes up to the absolute value of count occurrences starting from the tail of the
        ///     list.
        ///     - If count is 0, removes all occurrences of the specified value.
        /// </param>
        /// <param name="value">The value to remove from the list.</param>
        /// <returns>The number of removed occurrences of the specified value.</returns>
        public static int LRem(string key, int count, string value)
        {
            if (!_listStore.TryGetValue(key, out var list)) return 0;

            var removed = 0;

            switch (count)
            {
                case 0:
                    {
                        var node = list.First;
                        while (node != null)
                        {
                            var next = node.Next;
                            if (node.Value == value)
                            {
                                list.Remove(node);
                                removed++;
                            }

                            node = next;
                        }

                        break;
                    }

                case > 0:
                    {
                        var node = list.First;
                        while (node != null && removed < count)
                        {
                            var next = node.Next;
                            if (node.Value == value)
                            {
                                list.Remove(node);
                                removed++;
                            }

                            node = next;
                        }

                        break;
                    }

                case < 0:
                    {
                        var node = list.Last;
                        while (node != null && removed < Math.Abs(count))
                        {
                            var prev = node.Previous;
                            if (node.Value == value)
                            {
                                list.Remove(node);
                                removed++;
                            }

                            node = prev;
                        }

                        break;
                    }
            }

            return removed;
        }

        /// <summary>
        ///     Trims a list to retain only the elements within the specified range, removing elements outside the range.
        /// </summary>
        /// <param name="key">The key of the list to trim.</param>
        /// <param name="start">
        ///     The zero-based start index of the range to retain. Negative values indicate offsets from the end of
        ///     the list.
        /// </param>
        /// <param name="end">
        ///     The zero-based end index of the range to retain. Negative values indicate offsets from the end of the
        ///     list.
        /// </param>
        /// <returns>"OK" if the operation succeeds, or null if the key does not exist or the list is empty.</returns>
        public static string LTrim(string key, int start, int end)
        {
            if (!_listStore.TryGetValue(key, out var list)) return null;

            var count = list.Count;
            if (count == 0) return null;

            if (start < 0) start += count;
            if (end < 0) end += count;

            start = Math.Max(0, start);
            end = Math.Min(count - 1, end);

            if (start > end)
            {
                list.Clear();
                return "OK";
            }

            var index = 0;
            var node = list.First;
            while (node != null)
            {
                var next = node.Next;
                if (index < start || index > end) list.Remove(node);
                index++;
                node = next;
            }

            return "OK";
        }
    }
}