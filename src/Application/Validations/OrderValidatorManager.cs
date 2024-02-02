using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands.UpdateOrder;
using StoreOnline.Domain.Common;
using StoreOnline.Domain.Exceptions;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Validations;

public class OrderValidatorManager(ICustomerReadRepository customerReadRepository,
    IProductReadRepository productReadRepository) : IDomainValidator<IOrderCommand>
{
    public async Task<bool> Validate(IOrderCommand data)
    {
        CustomerExistsValidator customerExistsValidator = new(customerReadRepository);
        ProductExistsValidator productExistsValidator = new(productReadRepository);
        ProductOnStockValidator productOnStockValidator = new(productReadRepository);

        bool isCustomerValid = await customerExistsValidator.Validate(data);
        if (!isCustomerValid)
        {
            throw new CustomerNotFoundException("The Customer doesn't exists");
        }

        bool isProductExists = await productExistsValidator.Validate(data);
        if (!isProductExists)
        {
            throw new ProductNotFoundException("Any Product doesn't found");
        }

        if (data is UpdateOrderCommand)
        {
            return await Task.FromResult(true);
        }

        bool onStockProductValid = await productOnStockValidator.Validate(data);
        if (!onStockProductValid)
        {
            throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
        }
        return await Task.FromResult(true);
    }
}
