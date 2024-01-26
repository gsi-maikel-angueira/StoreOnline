using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

class CreateOrderServices(IApplicationDbContext applicationDbContext) : ICreateOrderServices<CreateOrderCommand>
{
    public Order CreateOrUpdate(CreateOrderCommand request)
    {
        Order entity = new()
        {
            CreatedDate = DateTime.Today,
            CustomerId = request.CustomerId,
            OrderNumber = Guid.NewGuid().ToString()
        };
        request.Products.ForEach(p =>
        {
            Product? currentProduct = applicationDbContext.Products.Find(p.ProductId);
            OrderDetail orderDetail = new()
            {
                Quantity = p.Quantity, 
                Order = entity, 
                Product = currentProduct
            };
            if (currentProduct != null)
            {
                currentProduct.Stock -= p.Quantity;
            }
            applicationDbContext.OrderDetails.Add(orderDetail);
        });
        applicationDbContext.Orders.Add(entity);
        return entity;
    }
}
