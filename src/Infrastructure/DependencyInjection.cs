using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreOnline.Application.Batch.Commands;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Repositories;
using StoreOnline.Infrastructure.Data;
using StoreOnline.Infrastructure.Data.Configurations;
using StoreOnline.Infrastructure.Repositories;

namespace StoreOnline.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlite(connectionString);
        });


        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<SaveJobTimerCallback>();
        services.AddScoped<ScheduleJobManager>();
        services.AddSingleton<IDbContextFactory, AppDbConnectionFactory>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        return services;
    }
}
