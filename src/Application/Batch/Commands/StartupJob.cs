using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Utils;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Batch.Commands;

public record ScheduleJobCommand(int TimeInMinutes) : IRequest<string>;

public class ScheduleJobHandler(ScheduleJobManager scheduleJobManager) : IRequestHandler<ScheduleJobCommand, string>
{
    public async Task<string> Handle(ScheduleJobCommand request, CancellationToken cancellationToken)
    {
        return await scheduleJobManager.StartJob(request.TimeInMinutes);
    }
}

public class ScheduleJobManager(SaveJobTimerCallback saveJobTimerCallback)
{
    
    private readonly Dictionary<string, Timer> RunningTimers = new();

    public Task<string> StartJob(int timeInMinutes)
    {
        Timer timer = new(saveJobTimerCallback.SaveJobFile, JobStatus.Running, TimeSpan.Zero, TimeSpan.FromMinutes(timeInMinutes));
        string key = Guid.NewGuid().ToString();
        RunningTimers.Add(key, timer);
        return Task.FromResult(key);
    }

    public Timer ReadRunningTimer(string key)
    {
        if (!RunningTimers.ContainsKey(key))
            throw new ArgumentException("Any timer was registered using this key.");
        return RunningTimers[key];
    }
}

public interface IDbContextFactory
{
    IApplicationDbContext NewContext();
}

public class SaveJobTimerCallback (IDbContextFactory contextFactory, ILogger<SaveJobTimerCallback> logger)
{
    private const string BatchFolderPath = "/BatchFileOutput/";
    private readonly string _batchOutputDirectoryPath = Directory.GetCurrentDirectory() + BatchFolderPath;  
    private const string BatchFileNameFormat = "BatchFile-{0}.txt";
    private static readonly object LockObject = new();
    private static readonly ConcurrentQueue<int> OrdersProcessedItems = new();
    private static int BatchSequenceNumber = 1;
    
    public void SaveJobFile(object? state)
    {
        string batchFileName = string.Format(BatchFileNameFormat, DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HH'-'mm'-'ss"));
        IApplicationDbContext applicationDbContext = contextFactory.NewContext();
        OrderUtil orderUtil = new(applicationDbContext);
        lock (LockObject)
        {
            List<Order> saveOrders;
            if (OrdersProcessedItems.IsEmpty)
            {
                saveOrders = applicationDbContext.Orders.ToList();
            }
            else
            {
                saveOrders = 
                    applicationDbContext.Orders
                        .Where(o => !OrdersProcessedItems.Contains(o.Id))
                        .ToList(); 
            }
            
            if(saveOrders.Count == 0) return;
            
            List<string> orderToSave = new() { $"Batch: {BatchSequenceNumber}" };
            foreach (Order order in saveOrders)
            {
                orderToSave.AddRange(orderUtil.ToString(order.Id));
                orderToSave.Add($"Total: {orderUtil.OrderTotalAmount(order.Id):C}");
            }

            if (!Directory.Exists(_batchOutputDirectoryPath))
            {
                Directory.CreateDirectory(_batchOutputDirectoryPath);
            }
            string pathToSave = _batchOutputDirectoryPath + batchFileName;
            logger.Log(LogLevel.Information, pathToSave);
            File.WriteAllLines(pathToSave, orderToSave);
            saveOrders.ForEach(o => OrdersProcessedItems.Enqueue(o.Id));
            BatchSequenceNumber += 1;
        }
    }
}

public enum JobStatus
{
    Stop, Running, Waiting
}


