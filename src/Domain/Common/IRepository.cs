namespace StoreOnline.Domain.Common;

public interface IRepository<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    void Remove(TEntity entity);
}
