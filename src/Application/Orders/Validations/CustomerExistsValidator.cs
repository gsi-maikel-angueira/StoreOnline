using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands;

namespace StoreOnline.Application.Orders.Validations;

public class CustomerExistsValidator(IApplicationDbContext applicationDbContext) : Domain.Common.IValidator<IOrderCommand>
{
    
    public bool Validate(IOrderCommand createOrderCommand)
    {
        return applicationDbContext.Customers.Any(c => c.Id == createOrderCommand.CustomerId);
    }
}
