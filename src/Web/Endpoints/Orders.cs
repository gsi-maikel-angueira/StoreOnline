using StoreOnline.Application.Orders.Commands.CreateOrder;
using StoreOnline.Application.Orders.Commands.UpdateOrder;
using StoreOnline.Web.Infrastructure;

namespace StoreOnline.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateOrder)
            .MapPut(UpdateOrder, "{id}");
    }

    private async Task<int> CreateOrder(ISender sender, CreateOrderCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<int> UpdateOrder(ISender sender, int id, UpdateOrderCommand command)
    {
        command.OrderId = id;
        return await sender.Send(command);
    }
}
