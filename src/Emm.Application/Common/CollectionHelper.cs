namespace Emm.Application.Common;

public static class CollectionHelper
{
    public static void RemoveItemsNotInRequest<T, TKey>(
    IReadOnlyCollection<T> collection,
    ISet<TKey> requestIds,
    Func<T, TKey> keySelector,
    Action<TKey> removeAction)
    where TKey : notnull
    {
        var idsToRemove = collection
            .Select(keySelector)
            .Where(id => !requestIds.Contains(id))
            .ToList();

        foreach (var id in idsToRemove)
        {
            removeAction(id);
        }
    }

}
