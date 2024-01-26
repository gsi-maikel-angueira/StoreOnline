using StoreOnline.Application.Orders.Commands.CreateOrder;

namespace StoreOnline.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(v => v.OrderId)
            .NotEmpty();
        RuleFor(v => v.CustomerId)
            .GreaterThan(0)
            .NotEmpty();
        RuleFor(v => v.Products)
            .NotEmpty();
    }
}

