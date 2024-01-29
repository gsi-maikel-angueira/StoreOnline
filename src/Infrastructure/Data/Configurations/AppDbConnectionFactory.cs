using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoreOnline.Application.Batch.Commands;
using StoreOnline.Application.Common.Interfaces;

namespace StoreOnline.Infrastructure.Data.Configurations;

public class AppDbConnectionFactory(IConfiguration configuration) : IDbContextFactory
{
    public IApplicationDbContext NewDbContext()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");
        DbContextOptionsBuilder<ApplicationDbContext> builder = new();
        builder.UseSqlite(connectionString);
        return new ApplicationDbContext(builder.Options);
    }
}
