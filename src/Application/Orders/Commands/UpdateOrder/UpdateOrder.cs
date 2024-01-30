using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Payloads;
using StoreOnline.Application.Services;
using StoreOnline.Application.Validations;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;
using StoreOnline.Domain.Repositories;

namespace StoreOnline.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand : IRequest<OrderVm>, IOrderCommand
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = new(); 
}

public class UpdateCommandHandler(
    IApplicationDbContext context, 
    UpdateOrderServices updateOrderServices,
    ICustomerReadRepository customerReadRepository, 
    IProductReadRepository productReadRepository) : IRequestHandler<UpdateOrderCommand, OrderVm>
{
    public async Task<OrderVm> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        CustomerExistsValidator customerExistsValidator = new(customerReadRepository);
        ProductOnStockValidator productOnStockValidator = new(productReadRepository);
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

        Order currentOrder = await updateOrderServices.CreateOrUpdateAsync(request);
        await context.SaveChangesAsync(cancellationToken);
        return new OrderVm { Id = currentOrder.Id, OrderNumber = currentOrder.OrderNumber };
    }
}
