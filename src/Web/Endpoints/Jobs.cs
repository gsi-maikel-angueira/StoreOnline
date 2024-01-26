using StoreOnline.Application.Batch.Commands;
using StoreOnline.Web.Infrastructure;

namespace StoreOnline.Web.Endpoints;

public class Jobs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(StartScheduleJob,"{timeInMinutes}");
    }
    
    public async Task<string> StartScheduleJob(ISender sender, int timeInMinutes)
    {
        return await sender.Send(new ScheduleJobCommand(timeInMinutes));
    }
}
