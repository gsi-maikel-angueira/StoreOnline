using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Infrastructure.Repositories;

public class OrderRepository(IApplicationDbContext applicationDbContext) : IOrderRepository
{
    public async Task<Order> AddAsync(Order entity)
    {
        EntityEntry<Order> entityEntry = await applicationDbContext.Orders.AddAsync(entity);
        return entityEntry.Entity;
    }

    public void Remove(Order entity)
    {
        applicationDbContext.Orders.Remove(entity);
    }

    public async Task<bool> ExistsAsync(int key)
    {
        return await applicationDbContext.Orders.AnyAsync(o => o.Id == key);
    }

    public async Task<Order?> FindByIdAsync(int key)
    {
        return await applicationDbContext.Orders.FindAsync(key);
    }

    public async Task<IEnumerable<Order>> FindAllAsync()
    {
        return await applicationDbContext.Orders.ToListAsync();
    }
}
