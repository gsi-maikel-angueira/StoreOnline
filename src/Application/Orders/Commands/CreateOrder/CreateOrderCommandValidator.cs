namespace StoreOnline.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(v => v.CustomerId)
            .GreaterThan(0)
            .NotEmpty();
        RuleFor(v => v.Products)
            .NotEmpty();
    }
}
