using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Orders.Validations;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<int>
{
    public int? OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = new(); 
}

public class CreateTodoItemCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Domain.Common.IValidator<CreateOrderCommand> customerExistsValidator = new CustomerExistsValidator(context);
        Domain.Common.IValidator<CreateOrderCommand> productOnStockValidator = new ProductOnStockValidator(context);
        if (!customerExistsValidator.Validate(request))
        {
            throw new CustomerNotFoundException("Customer doesn't exists");
        }

        if (!productOnStockValidator.Validate(request))
        {
            throw new ProductExceedLimitOnStockException("Product exceed the limit on stock");
        }

        CreateOrderServiceFactory orderServiceFactory = new(context);
        ICreateOrderServices createOrderServices = orderServiceFactory.Create(request);
        Order currentOrder = createOrderServices.CreateOrUpdate(request);
        await context.SaveChangesAsync(cancellationToken);
        return currentOrder.Id;
    }
}
