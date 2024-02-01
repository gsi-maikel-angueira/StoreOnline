using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Domain.Common;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Validations;

public class ProductExistsValidator(IProductReadRepository productReadRepository) : IDomainValidator<IOrderCommand>
{
    public async Task<bool> Validate(IOrderCommand createOrderCommand)
    {
        foreach (ProductDto dto in createOrderCommand.Products)
        {
            bool existsAsync = await productReadRepository.ExistsAsync(dto.ProductId);
            if (!existsAsync)
            {
                return await Task.FromResult(false);
            }
        }

        return await Task.FromResult(true);
    }
}
