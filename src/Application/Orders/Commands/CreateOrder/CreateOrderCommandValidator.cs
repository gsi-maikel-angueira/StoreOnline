using StoreOnline.Application.Common.Models;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(v => v.CustomerId)
            .GreaterThan(0)
            .NotEmpty();
        RuleForEach(v => v.Products)
            .NotEmpty()
            .SetValidator(new ProductValidator());
    }
}

public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(p => p.Quantity).GreaterThan(0);
    }
}
