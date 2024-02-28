namespace StoreOnline.Domain.Extensions;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this ICollection<T> items)
    {
        return items.Count == 0;
    }
}
