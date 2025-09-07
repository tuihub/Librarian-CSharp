namespace Librarian.Common.Helpers;

public static class CollectionHelper
{
    /// <summary>
    ///     Compares two collections and returns the items to be removed, added, and updated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="oldCollection"></param>
    /// <param name="newCollection"></param>
    /// <param name="keySelector"></param>
    /// <param name="contentComparer">Function to compare two items. If null, default equality comparer will be used.</param>
    /// <returns></returns>
    public static (ICollection<T> ToRemove, ICollection<T> ToAdd, ICollection<(T OldItem, T NewItem)> ToUpdate)
        CompareCollections<T, TKey>(
            ICollection<T> oldCollection,
            ICollection<T> newCollection,
            Func<T, TKey> keySelector,
            Func<T, T, bool>? contentComparer = null)
        where TKey : notnull
    {
        contentComparer ??= (l, r) => l!.Equals(r);

        var oldDict = oldCollection.ToDictionary(keySelector);
        var newDict = newCollection.ToDictionary(keySelector);

        var toRemove = oldCollection
            .Where(item => !newDict.ContainsKey(keySelector(item)))
            .ToList();

        var toAdd = newCollection
            .Where(item => !oldDict.ContainsKey(keySelector(item)))
            .ToList();

        var toUpdate = new List<(T OldItem, T NewItem)>();
        foreach (var newItem in newCollection)
        {
            var key = keySelector(newItem);
            if (oldDict.TryGetValue(key, out var oldItem) && !contentComparer(oldItem, newItem))
                toUpdate.Add((oldItem, newItem));
        }

        return (toRemove, toAdd, toUpdate);
    }

    /// <summary>
    ///     Compares two collections of different types and returns the items to be removed, added, and updated.
    /// </summary>
    /// <typeparam name="TOld">Type of items in the old collection</typeparam>
    /// <typeparam name="TNew">Type of items in the new collection</typeparam>
    /// <typeparam name="TKey">Type of the key used for comparison</typeparam>
    /// <param name="oldCollection">The original collection</param>
    /// <param name="newCollection">The new collection to compare against</param>
    /// <param name="oldKeySelector">Function to extract the key from old collection items</param>
    /// <param name="newKeySelector">Function to extract the key from new collection items</param>
    /// <param name="newToOldConverter">Function to convert new items to old items type</param>
    /// <param name="contentComparer">
    ///     Function to compare old and converted new items. If null, default equality comparer will
    ///     be used.
    /// </param>
    /// <returns>Tuple containing collections of items to remove, add, and update</returns>
    public static (ICollection<TOld> ToRemove, ICollection<TOld> ToAdd, ICollection<(TOld OldItem, TNew NewItem)>
        ToUpdate)
        CompareCollections<TOld, TNew, TKey>(
            ICollection<TOld> oldCollection,
            ICollection<TNew> newCollection,
            Func<TOld, TKey> oldKeySelector,
            Func<TNew, TKey> newKeySelector,
            Func<TNew, TOld> newToOldConverter,
            Func<TOld, TOld, bool>? contentComparer = null)
        where TKey : notnull
    {
        contentComparer ??= (l, r) => l!.Equals(r);

        var oldDict = oldCollection.ToDictionary(oldKeySelector);
        var newKeys = newCollection.Select(newKeySelector).ToHashSet();

        var toRemove = oldCollection
            .Where(item => !newKeys.Contains(oldKeySelector(item)))
            .ToList();

        var toAdd = newCollection
            .Where(item => !oldDict.ContainsKey(newKeySelector(item)))
            .Select(newToOldConverter)
            .ToList();

        var toUpdate = new List<(TOld OldItem, TNew NewItem)>();
        foreach (var newItem in newCollection)
        {
            var key = newKeySelector(newItem);
            if (oldDict.TryGetValue(key, out var oldItem))
            {
                var convertedNewItem = newToOldConverter(newItem);
                if (!contentComparer(oldItem, convertedNewItem)) toUpdate.Add((oldItem, newItem));
            }
        }

        return (toRemove, toAdd, toUpdate);
    }
}