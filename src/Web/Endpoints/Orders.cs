using StoreOnline.Application.Orders.Commands.CreateOrder;
using StoreOnline.Web.Infrastructure;

namespace StoreOnline.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateOrder);
    }
    
    public async Task<int> CreateOrder(ISender sender, CreateOrderCommand command)
    {
        return await sender.Send(command);
    }
}
