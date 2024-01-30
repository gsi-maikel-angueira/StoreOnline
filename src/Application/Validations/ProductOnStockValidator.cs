using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Validations;

public class ProductOnStockValidator(IProductReadRepository productReadRepository) : Domain.Common.IValidator<IOrderCommand>
{
    public async Task<bool> Validate(IOrderCommand createOrderCommand)
    {
        foreach (ProductDto dto in createOrderCommand.Products)
        {
            Product? currentProduct = await productReadRepository.FindByIdAsync(dto.ProductId);
            bool isStockLess = (currentProduct?.Stock ?? 0) < dto.Quantity;
            if (isStockLess)
            {
                return await Task.FromResult(false);
            }
        }

        return await Task.FromResult(true);
    }
}
