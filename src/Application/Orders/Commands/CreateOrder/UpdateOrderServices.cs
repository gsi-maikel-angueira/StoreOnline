using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

class UpdateOrderServices(IApplicationDbContext applicationDbContext) : ICreateOrderServices
{
    public Order CreateOrUpdate(CreateOrderCommand request)
    {
        Order? currentOrder = applicationDbContext.Orders.Find(request.OrderId);
        if (currentOrder == null)
        {
            throw new OrderNotFoundException("Order not found.");
        }
        if (currentOrder.CustomerId != request.CustomerId)
        {
            throw new UnsupportedOrderException("The Order cannot be change the customer");
        }
        
        AddOrUpdateOrder(request, currentOrder);
        CheckAndDeleteProducts(request, currentOrder);
        return currentOrder;
    }

    private void CheckAndDeleteProducts(CreateOrderCommand request, Order currentOrder)
    {
        List<OrderDetail> deletedOrderDetails = new();
        List<OrderDetail> orderDetails = applicationDbContext.OrderDetails.Where(order => order.OrderId == currentOrder.Id).ToList();
        orderDetails.ForEach(orderDetail =>
        {
            if (request.Products.All(p => orderDetail.ProductId != p.ProductId))
            {
                Product? currentProduct = applicationDbContext.Products.Find(orderDetail.ProductId);
                if (currentProduct != null)
                {
                    currentProduct.Stock += orderDetail.Quantity;
                }

                deletedOrderDetails.Add(orderDetail);
            }
        });
        deletedOrderDetails.ForEach(orderDetail =>
        {
            applicationDbContext.OrderDetails.Remove(orderDetail);
        });
    }

    private void AddOrUpdateOrder(CreateOrderCommand request, Order currentOrder)
    {
        request.Products.ForEach(productDto =>
        {
            OrderDetail? orderDetail = currentOrder.OrderDetails.Find(o => o.ProductId == productDto.ProductId);
            Product? currentProduct = applicationDbContext.Products.Find(productDto.ProductId);
            if (orderDetail == null)
            {
                OrderDetail newOrderDetails = new()
                {
                    Quantity = productDto.Quantity, 
                    Order = currentOrder, 
                    Product = currentProduct
                };
                if (currentProduct != null)
                {
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
                    currentProduct.Stock += (savedQuantity - newQuantity);
                }
                else
                {
                    int value = newQuantity - savedQuantity;
                    if (currentProduct.Stock < value)
                    {
                        throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
                    }

                    currentProduct.Stock -= value;
                }
            }
        });
    }
}
