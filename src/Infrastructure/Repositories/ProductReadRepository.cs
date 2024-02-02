using Microsoft.EntityFrameworkCore;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Infrastructure.Repositories;

public class ProductReadRepository(IApplicationDbContext applicationDbContext) : IProductReadRepository
{
    public async Task<bool> ExistsAsync(int key) => await applicationDbContext.Products.AnyAsync(p => p.Id == key);

    public async Task<Product?> FindByIdAsync(int key) => await applicationDbContext.Products.FindAsync(key);

    public async Task<IEnumerable<Product>> FindAllAsync() => await applicationDbContext.Products.ToListAsync();
}
