using Microsoft.EntityFrameworkCore;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Infrastructure.Repositories;

public class CustomerReadRepository(IApplicationDbContext applicationDbContext) : ICustomerReadRepository
{
    public async Task<bool> ExistsAsync(int key)
    {
        return await applicationDbContext.Customers.AnyAsync(c => c.Id == key);
    }

    public async Task<Customer?> FindByIdAsync(int key)
    {
        return await applicationDbContext.Customers.FindAsync(key);
    }

    public async Task<IEnumerable<Customer>> FindAllAsync()
    {
        return await applicationDbContext.Customers.ToListAsync();
    }
}
