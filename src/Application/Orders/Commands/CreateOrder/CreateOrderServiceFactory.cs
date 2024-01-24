using StoreOnline.Application.Common.Interfaces;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

class CreateOrderServiceFactory(IApplicationDbContext applicationDbContext)
{
    public ICreateOrderServices Create(CreateOrderCommand createOrderCommand)
    {
        return createOrderCommand.OrderId == null
            ? new CreateOrderServices(applicationDbContext)
            : new UpdateOrderServices(applicationDbContext);
    }
}
