using Microsoft.EntityFrameworkCore;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Infrastructure.Repositories;

public class CustomerReadRepository(IApplicationDbContext applicationDbContext) : ICustomerReadRepository
{
    public async Task<bool> ExistsAsync(int key) => await applicationDbContext.Customers.AnyAsync(c => c.Id == key);

    public async Task<Customer?> FindSingleAsync(int key) => await applicationDbContext.Customers.FindAsync(key);

    public async Task<IEnumerable<Customer>> FindAllAsync() => await applicationDbContext.Customers.ToListAsync();
}
