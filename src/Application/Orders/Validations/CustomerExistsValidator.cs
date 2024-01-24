using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands.CreateOrder;

namespace StoreOnline.Application.Orders.Validations;

public class CustomerExistsValidator(IApplicationDbContext applicationDbContext) : Domain.Common.IValidator<CreateOrderCommand>
{
    
    public bool Validate(CreateOrderCommand createOrderCommand)
    {
        return applicationDbContext.Customers.Any(c => c.Id == createOrderCommand.CustomerId);
    }
}
