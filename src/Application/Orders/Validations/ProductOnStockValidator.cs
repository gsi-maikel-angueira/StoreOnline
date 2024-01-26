using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands;

namespace StoreOnline.Application.Orders.Validations;

public class ProductOnStockValidator(IApplicationDbContext applicationDbContext) : Domain.Common.IValidator<IOrderCommand>
{
    public bool Validate(IOrderCommand createOrderCommand)
    {
        return createOrderCommand.Products.All(
            p => applicationDbContext.Products.Find(p.ProductId)?.Stock > p.Quantity);
    }
}
