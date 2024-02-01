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
        var savedOrder = await orderRepository.AddAsync(newOrder);
        return savedOrder;

        async void AddOrderAsync(ProductDto p)
        {
            Product? currentProduct = await productReadRepository.FindByIdAsync(p.ProductId);
            if (currentProduct == null) throw new ProductNotFoundException("Product not found.");
            OrderDetail orderDetail = new() { Quantity = p.Quantity, Order = newOrder, Product = currentProduct };
            currentProduct.Stock -= p.Quantity;
            newOrder.OrderDetails.Add(orderDetail);
            await orderDetailRepository.AddAsync(orderDetail);
        }
    }
}

public static class RandomGenerator
{
    public static string NewKey()
    {
        return Guid.NewGuid().ToString("n")[..8];
    }
}
