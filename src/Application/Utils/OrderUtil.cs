using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Utils;

public class OrderUtil (IApplicationDbContext applicationDbContext)
{

    public double OrderTotalAmount(int orderId)
    {
        return applicationDbContext.OrderDetails
            .Where(o => o.OrderId == orderId)
            .Select(od => od.Quantity * od.Product!.Price)
            .Sum();
    }

    public List<string> ToString(int orderId)
    {
        List <string> result = new();
        if(!applicationDbContext.Orders.Any(o => o.Id == orderId))
            return result;
        result.AddRange(
            applicationDbContext.OrderDetails.Where(od => od.OrderId == orderId).Select(
                orderDetail => string.Format("#:{0}   P:{1}   A:{2:C}   Buyer:{3}", 
                    orderDetail.Order!.OrderNumber, 
                    orderDetail.ProductId, 
                    (orderDetail.Quantity * orderDetail.Product!.Price), 
                    orderDetail.Order!.Customer!.Nid)));
        return result;
    }
}
