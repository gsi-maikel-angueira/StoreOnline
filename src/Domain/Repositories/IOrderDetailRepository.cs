using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Domain.Repositories;

public interface IOrderDetailRepository : IRepository<OrderDetail>, IReadRepository<int, OrderDetail>
{
    public Task<OrderDetail?> FindByKeys(int orderId, int productId);
    public Task<List<OrderDetail>> FindByOrderAsync(int orderId);
}
