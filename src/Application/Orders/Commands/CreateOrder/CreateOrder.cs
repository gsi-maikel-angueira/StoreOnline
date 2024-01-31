using Microsoft.Extensions.DependencyInjection;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Payloads;
using StoreOnline.Application.Services;
using StoreOnline.Application.Validations;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<OrderVm>, IOrderCommand
{
    public int CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}

public class CreateOrderCommandHandler(
        IApplicationDbContext context,
        CustomerExistsValidator customerExistsValidator,
        ProductOnStockValidator productOnStockValidator,
        [FromKeyedServices(nameof(CreateOrderServices))] ICreateOrderServices<CreateOrderCommand> createOrderServices)
    : IRequestHandler<CreateOrderCommand, OrderVm>
{
    public async Task<OrderVm> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        bool isCustomerValid = await customerExistsValidator.Validate(request);
        if (!isCustomerValid)
        {
            throw new CustomerNotFoundException("Customer doesn't exists");
        }

        bool onStockProductValid = await productOnStockValidator.Validate(request);
        if (!onStockProductValid)
        {
            throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
        }

        Order currentOrder = await createOrderServices.CreateOrUpdateAsync(request);
        await context.SaveChangesAsync(cancellationToken);
        return new OrderVm { Id = currentOrder.Id, OrderNumber = currentOrder.OrderNumber };
    }
}
