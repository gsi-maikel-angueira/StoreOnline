using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Infrastructure.Repositories;

public class OrderDetailRepository(IApplicationDbContext applicationDbContext) : IOrderDetailRepository
{
    public async Task<OrderDetail> AddAsync(OrderDetail entity)
    {
        EntityEntry<OrderDetail> entityEntry = await applicationDbContext.OrderDetails.AddAsync(entity);
        return entityEntry.Entity;
    }

    public void Remove(OrderDetail entity) => applicationDbContext.OrderDetails.Remove(entity);

    public async Task<bool> ExistsAsync(int key) => await applicationDbContext.OrderDetails.AnyAsync(od => od.Id == key);

    public async Task<OrderDetail?> FindSingleAsync(int orderId, int productId) =>
        await applicationDbContext.OrderDetails
            .Where(od => od.OrderId == orderId && od.ProductId == productId)
            .FirstOrDefaultAsync();

    public async Task<List<OrderDetail>> FindAsync(int orderId) => await applicationDbContext.OrderDetails.Where(od => od.OrderId == orderId).ToListAsync();

    public async Task<OrderDetail?> FindSingleAsync(int key) => await applicationDbContext.OrderDetails.FindAsync(key);

    public async Task<IEnumerable<OrderDetail>> FindAllAsync() => await applicationDbContext.OrderDetails.ToListAsync();
}
