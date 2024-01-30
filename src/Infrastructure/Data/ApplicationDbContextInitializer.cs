using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitialiseAsync();

        await initializer.SeedAsync();
    }
}

public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        if (!context.Customers.Any())
        {
            context.Customers.Add(new Customer()
            {
                Nid = "81100300226",
                FirstName = "Michael",
                LastName = "Jackson",
                CardNumber = "6945-5845-5214-4741"
            });

            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var product0 = new Product() { Name = "Phone Samsumg", Price = 125.25, Stock = 150 };
            var product1 = new Product() { Name = "Mouse", Price = 12.52, Stock = 25 };
            var product2 = new Product() { Name = "Flash Memory", Price = 25.00, Stock = 35 };
            var product3 = new Product() { Name = "Adapter USB", Price = 5.85, Stock = 45 };
            context.Products.Add(product0);
            context.Products.Add(product1);
            context.Products.Add(product2);
            context.Products.Add(product3);

            await context.SaveChangesAsync();
        }
    }
}
