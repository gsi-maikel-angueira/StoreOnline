namespace StoreOnline.Domain.Common;

public interface IReadRepository<in TKey, TEntity> where TEntity : class
{
    Task<bool> ExistsAsync(TKey key);
    Task<TEntity?> FindSingleAsync(TKey key);
    Task<IEnumerable<TEntity>> FindAllAsync();
}
