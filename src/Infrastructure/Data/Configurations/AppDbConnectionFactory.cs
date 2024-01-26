using Microsoft.EntityFrameworkCore;
using StoreOnline.Application.Batch.Commands;
using StoreOnline.Application.Common.Interfaces;

namespace StoreOnline.Infrastructure.Data.Configurations;

public class AppDbConnectionFactory : IDbContextFactory
{
    public IApplicationDbContext NewContext()
    {
        DbContextOptionsBuilder<ApplicationDbContext> builder = new();
        builder.UseSqlite("DataSource=app.db;Cache=Shared");
        return new ApplicationDbContext(builder.Options);
    }
}
