using Microsoft.Extensions.DependencyInjection;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Payloads;
using StoreOnline.Application.Services;
using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand : IRequest<OrderVm>, IOrderCommand
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}

public class UpdateCommandHandler(
        IApplicationDbContext context,
        IDomainValidator<IOrderCommand> orderValidatorManager,
        [FromKeyedServices(nameof(UpdateOrderServices))]
        ICreateOrderServices<UpdateOrderCommand> updateOrderServices)
    : IRequestHandler<UpdateOrderCommand, OrderVm>
{
    public async Task<OrderVm> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var isValid = await orderValidatorManager.Validate(request);
        Order currentOrder = await updateOrderServices.CreateOrUpdateAsync(request);
        await context.SaveChangesAsync(cancellationToken);
        return new OrderVm { Id = currentOrder.Id, OrderNumber = currentOrder.OrderNumber };
    }
}
