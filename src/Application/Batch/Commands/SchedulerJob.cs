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
        Timer timer = new(saveJobTimerCallback.SaveJobFile, JobStatus.Running, TimeSpan.Zero,
            TimeSpan.FromMinutes(timeInMinutes));
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
    IApplicationDbContext NewDbContext();
}

public class SaveJobTimerCallback(IDbContextFactory contextFactory, ILogger<SaveJobTimerCallback> logger)
{
    private const string BatchFolderPath = "/BatchFileOutput/";
    private readonly string _batchOutputDirectoryPath = Directory.GetCurrentDirectory() + BatchFolderPath;
    private const string BatchFileNameFormat = "BatchFile-{0}.txt";
    private static readonly object LockObject = new();
    private static readonly ConcurrentQueue<int> OrdersProcessedItems = new();
    private static int s_batchSequenceNumber = 1;

    public void SaveJobFile(object? state)
    {
        string batchFileName =
            string.Format(BatchFileNameFormat, DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HH'-'mm'-'ss"));
        IApplicationDbContext applicationDbContext = contextFactory.NewDbContext();
        OrderUtil orderUtil = new(applicationDbContext);
        lock (LockObject)
        {
            List<Order> fileWriteOrders;
            if (OrdersProcessedItems.IsEmpty)
            {
                fileWriteOrders = applicationDbContext.Orders.ToList();
            }
            else
            {
                fileWriteOrders =
                    applicationDbContext.Orders
                        .Where(o => !OrdersProcessedItems.Contains(o.Id))
                        .ToList();
            }

            if (fileWriteOrders.Count == 0) return;

            List<string> orderToSave = new() { $"Batch: {s_batchSequenceNumber}" };
            foreach (Order order in fileWriteOrders)
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
            fileWriteOrders.ForEach(o => OrdersProcessedItems.Enqueue(o.Id));
            s_batchSequenceNumber += 1;
        }
    }
}

public enum JobStatus
{
    Stop, Running, Waiting
}
