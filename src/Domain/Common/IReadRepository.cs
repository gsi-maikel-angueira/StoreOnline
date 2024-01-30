using StoreOnline.Domain.Entities;

namespace StoreOnline.Domain.Common;

public interface IReadRepository<in TKey, TEntity>
{
    Task<bool> ExistsAsync(TKey key);
    Task<TEntity?> FindByIdAsync(TKey key);
    Task<IEnumerable<TEntity>> FindAllAsync();
}
