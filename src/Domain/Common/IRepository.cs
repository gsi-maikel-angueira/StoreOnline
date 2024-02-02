namespace StoreOnline.Domain.Common;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    void Remove(TEntity entity);
}
