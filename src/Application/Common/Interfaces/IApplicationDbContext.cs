using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderDetail> OrderDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
