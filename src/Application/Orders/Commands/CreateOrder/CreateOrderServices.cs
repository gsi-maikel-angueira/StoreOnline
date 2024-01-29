using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

class CreateOrderServices(IApplicationDbContext applicationDbContext) : ICreateOrderServices<CreateOrderCommand>
{
    public async Task<Order> CreateOrUpdateAsync(CreateOrderCommand request)
    {
        Order newOrder = new()
        {
            CreatedDate = DateTime.Today,
            CustomerId = request.CustomerId,
            OrderNumber = RandomGenerator.NewKey()
        };

        request.Products.ForEach(AddOrderAsync);
        await applicationDbContext.Orders.AddAsync(newOrder);
        return newOrder;

        async void AddOrderAsync(ProductDto p)
        {
            Product? currentProduct = await applicationDbContext.Products.FindAsync(p.ProductId);
            if (currentProduct == null) throw new ProductNotFoundException("Product not found.");
            OrderDetail orderDetail = new() { Quantity = p.Quantity, Order = newOrder, Product = currentProduct };
            currentProduct.Stock -= p.Quantity;
            await applicationDbContext.OrderDetails.AddAsync(orderDetail);
        }
    }
}

static class RandomGenerator
{
    public static string NewKey()
    {
        return Guid.NewGuid().ToString("n")[..8];
    }
}
