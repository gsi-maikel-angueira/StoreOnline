using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Orders.Commands.CreateOrder;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Services;

public class CreateOrderServices(
        IOrderRepository orderRepository,
        IProductReadRepository productReadRepository,
        IOrderDetailRepository orderDetailRepository)
    : ICreateOrderServices<CreateOrderCommand>
{
    public async Task<Order> CreateOrUpdateAsync(CreateOrderCommand request)
    {
        Order newOrder = new()
        {
            CreatedDate = DateTime.Today, CustomerId = request.CustomerId, OrderNumber = RandomGenerator.NewKey()
        };

        request.Products.ForEach(AddOrderAsync);
        await orderRepository.AddAsync(newOrder);
        return newOrder;

        async void AddOrderAsync(ProductDto p)
        {
            Product? currentProduct = await productReadRepository.FindByIdAsync(p.ProductId);
            if (currentProduct == null) throw new ProductNotFoundException("Product not found.");
            OrderDetail orderDetail = new() { Quantity = p.Quantity, Order = newOrder, Product = currentProduct };
            currentProduct.Stock -= p.Quantity;
            await orderDetailRepository.AddAsync(orderDetail);
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
