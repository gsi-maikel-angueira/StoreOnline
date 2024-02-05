using Microsoft.Extensions.DependencyInjection;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Payloads;
using StoreOnline.Application.Services;
using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<OrderVm>, IOrderCommand
{
    public int CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}

public class CreateOrderCommandHandler(
        IApplicationDbContext context,
        IDomainValidator<IOrderCommand> orderValidatorManager,
        [FromKeyedServices(nameof(CreateOrderServices))] ICreateOrderServices<CreateOrderCommand> createOrderServices)
    : IRequestHandler<CreateOrderCommand, OrderVm>
{
    public async Task<OrderVm> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var isValid = await orderValidatorManager.Validate(request);
        Order currentOrder = await createOrderServices.CreateOrUpdateAsync(request);
        await context.SaveChangesAsync(cancellationToken);
        return new OrderVm { Id = currentOrder.Id, OrderNumber = currentOrder.OrderNumber };
    }
}
