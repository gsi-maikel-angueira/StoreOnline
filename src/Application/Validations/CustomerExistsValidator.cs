using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Domain.Common;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Validations;

public class CustomerExistsValidator(ICustomerReadRepository customerReadRepository) : IDomainValidator<IOrderCommand>
{
    public Task<bool> Validate(IOrderCommand createOrderCommand)
    {
        return customerReadRepository.ExistsAsync(createOrderCommand.CustomerId);
    }
}
