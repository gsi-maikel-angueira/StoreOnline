using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands.CreateOrder;

namespace StoreOnline.Application.Orders.Validations;

public class ProductOnStockValidator(IApplicationDbContext applicationDbContext) : Domain.Common.IValidator<CreateOrderCommand>
{
    public bool Validate(CreateOrderCommand createOrderCommand)
    {
        return createOrderCommand.Products.All(
            p => applicationDbContext.Products.Find(p.ProductId)?.Stock > p.Quantity);
    }
}
