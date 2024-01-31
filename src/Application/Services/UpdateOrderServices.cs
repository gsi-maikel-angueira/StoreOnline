using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Orders.Commands.UpdateOrder;
using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Services;

public class UpdateOrderServices(
    IOrderRepository orderRepository,
    IProductReadRepository productReadRepository,
    IOrderDetailRepository orderDetailRepository) : ICreateOrderServices<UpdateOrderCommand>
{
    public async Task<Order> CreateOrUpdateAsync(UpdateOrderCommand request)
    {
        Order? currentOrder = await orderRepository.FindByIdAsync(request.OrderId);
        if (currentOrder == null)
        {
            throw new OrderNotFoundException("Order not found.");
        }
        if (currentOrder.CustomerId != request.CustomerId)
        {
            throw new UnsupportedOrderException("The Order's customer cannot be changed");
        }

        var addOrUpdateOrderTask = AddOrUpdateOrderProductAsync(request, currentOrder);
        var deleteOrderTask = DeleteOrderProductsAsync(request, currentOrder);
        await addOrUpdateOrderTask;
        await deleteOrderTask;
        return currentOrder;
    }

    private async Task DeleteOrderProductsAsync(IOrderCommand request, BaseEntity currentOrder)
    {
        List<OrderDetail> deletedOrderDetails = new();
        var orderDetails = await orderDetailRepository.FindByOrderAsync(currentOrder.Id);

        orderDetails.ForEach(FindDeletedProductAsync);
        deletedOrderDetails.ForEach(orderDetail =>
        {
            orderDetailRepository.Remove(orderDetail);
        });
        return;

        async void FindDeletedProductAsync(OrderDetail orderDetail)
        {
            if (request.Products.Any(p => orderDetail.ProductId == p.ProductId))
            {
                return;
            }

            Product? currentProduct = await productReadRepository.FindByIdAsync(orderDetail.ProductId);
            if (currentProduct != null)
            {
                currentProduct.Stock += orderDetail.Quantity;
            }
            deletedOrderDetails.Add(orderDetail);
        }
    }

    private Task AddOrUpdateOrderProductAsync(IOrderCommand request, Order currentOrder)
    {
        request.Products.ForEach(UpdateOrderAsync);
        return Task.CompletedTask;

        async void UpdateOrderAsync(ProductDto productDto)
        {
            OrderDetail? orderDetail = await orderDetailRepository.FindByKeys(currentOrder.Id, productDto.ProductId);
            Product? currentProduct = await productReadRepository.FindByIdAsync(productDto.ProductId);
            if (orderDetail == null)
            {
                OrderDetail newOrderDetails = new() { Quantity = productDto.Quantity, Order = currentOrder, Product = currentProduct };
                if (currentProduct != null)
                {
                    if (currentProduct.Stock < productDto.Quantity)
                    {
                        throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
                    }

                    currentProduct.Stock -= productDto.Quantity;
                }

                currentOrder.OrderDetails.Add(newOrderDetails);
            }
            else
            {
                int savedQuantity = orderDetail.Quantity;
                int newQuantity = productDto.Quantity;
                if (currentProduct == null) return;

                if (savedQuantity > newQuantity)
                {
                    orderDetail.Quantity = newQuantity;
                    currentProduct.Stock += (savedQuantity - newQuantity);
                }
                else
                {
                    int value = newQuantity - savedQuantity;
                    if (currentProduct.Stock < value)
                    {
                        throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
                    }

                    orderDetail.Quantity = newQuantity;
                    currentProduct.Stock -= value;
                }
            }
        }
    }
}
