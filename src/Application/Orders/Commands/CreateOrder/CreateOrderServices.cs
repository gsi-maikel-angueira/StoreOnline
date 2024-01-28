using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

class CreateOrderServices(IApplicationDbContext applicationDbContext) : ICreateOrderServices<CreateOrderCommand>
{
    public async Task<Order> CreateOrUpdateAsync(CreateOrderCommand request)
    {
        Order entity = new()
        {
            CreatedDate = DateTime.Today,
            CustomerId = request.CustomerId,
            OrderNumber = RandomGenerator.NewKey()
        };

        async void AddOrderAsync(ProductDto p)
        {
            Product? currentProduct = await applicationDbContext.Products.FindAsync(p.ProductId);
            OrderDetail orderDetail = new() { Quantity = p.Quantity, Order = entity, Product = currentProduct };
            if (currentProduct != null)
            {
                currentProduct.Stock -= p.Quantity;
            }

            await applicationDbContext.OrderDetails.AddAsync(orderDetail);
        }

        request.Products.ForEach(AddOrderAsync);
        await applicationDbContext.Orders.AddAsync(entity);
        return entity;
    }
}

static class RandomGenerator
{
    public static string NewKey()
    {
        return Guid.NewGuid().ToString("n")[..8];
    }
}
