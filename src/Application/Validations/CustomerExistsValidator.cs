using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Validations;

public class CustomerExistsValidator(ICustomerReadRepository customerReadRepository) : Domain.Common.IValidator<IOrderCommand>
{
    public Task<bool> Validate(IOrderCommand createOrderCommand)
    {
        return customerReadRepository.ExistsAsync(createOrderCommand.CustomerId);
    }
}
