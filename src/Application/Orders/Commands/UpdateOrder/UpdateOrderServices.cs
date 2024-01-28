using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;

namespace StoreOnline.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderServices(IApplicationDbContext applicationDbContext) : ICreateOrderServices<UpdateOrderCommand>
{
    public async Task<Order> CreateOrUpdateAsync(UpdateOrderCommand request)
    {
        Order? currentOrder = await applicationDbContext.Orders.FindAsync(request.OrderId);
        if (currentOrder == null)
        {
            throw new OrderNotFoundException("Order not found.");
        }
        if (currentOrder.CustomerId != request.CustomerId)
        {
            throw new UnsupportedOrderException("The Order's customer cannot be changed");
        }

        await AddOrUpdateOrderProductAsync(request, currentOrder);
        await DeleteOrderProductsAsync(request, currentOrder);
        return currentOrder;
    }

    private async Task DeleteOrderProductsAsync(UpdateOrderCommand request, Order currentOrder)
    {
        List<OrderDetail> deletedOrderDetails = new();
        List<OrderDetail> orderDetails = await applicationDbContext.OrderDetails.Where(order => order.OrderId == currentOrder.Id).ToListAsync();
        orderDetails.ForEach(async orderDetail =>
        {
            if (request.Products.Any(p => orderDetail.ProductId == p.ProductId))
            {
                return;
            }

            Product? currentProduct = await applicationDbContext.Products.FindAsync(orderDetail.ProductId);
            if (currentProduct != null)
            {
                currentProduct.Stock += orderDetail.Quantity;
            }
            deletedOrderDetails.Add(orderDetail);
        });
        deletedOrderDetails.ForEach(orderDetail =>
        {
            applicationDbContext.OrderDetails.Remove(orderDetail);
        });
    }

    private Task AddOrUpdateOrderProductAsync(UpdateOrderCommand request, Order currentOrder)
    {
        async void UpdateOrderAsync(ProductDto productDto)
        {
            OrderDetail? orderDetail = await applicationDbContext.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == currentOrder.Id && od.ProductId == productDto.ProductId);
            Product? currentProduct = await applicationDbContext.Products.FindAsync(productDto.ProductId);
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

        request.Products.ForEach(UpdateOrderAsync);
        return Task.CompletedTask;
    }
}
