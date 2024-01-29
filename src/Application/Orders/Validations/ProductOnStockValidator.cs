using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands;

namespace StoreOnline.Application.Orders.Validations;

public class ProductOnStockValidator(IApplicationDbContext applicationDbContext) : Domain.Common.IValidator<IOrderCommand>
{
    public bool Validate(IOrderCommand createOrderCommand)
    {
        return createOrderCommand.Products
            .Select(dto => new 
                { 
                    OrderQuantity = dto.Quantity,
                    Stock = applicationDbContext.Products.Find(dto.ProductId)?.Stock ?? 0
                })
            .All(obj =>  obj.Stock >= obj.OrderQuantity);
    }
}
