using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Orders.Validations;
using StoreOnline.Application.Payloads;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<OrderVm>, IOrderCommand
{
    public int CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = new(); 
}

public class CreateOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateOrderCommand, OrderVm>
{
    public async Task<OrderVm> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Domain.Common.IValidator<IOrderCommand> customerExistsValidator = new CustomerExistsValidator(context);
        Domain.Common.IValidator<IOrderCommand> productOnStockValidator = new ProductOnStockValidator(context);
        if (!customerExistsValidator.Validate(request))
        {
            throw new CustomerNotFoundException("Customer doesn't exists");
        }

        if (!productOnStockValidator.Validate(request))
        {
            throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
        }

        CreateOrderServices createOrderServices = new(context);
        Order currentOrder = createOrderServices.CreateOrUpdate(request);
        await context.SaveChangesAsync(cancellationToken);
        return new OrderVm { Id = currentOrder.Id, OrderNumber = currentOrder.OrderNumber };
    }
}
