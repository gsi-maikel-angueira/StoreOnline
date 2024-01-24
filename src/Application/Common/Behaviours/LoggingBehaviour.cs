using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using StoreOnline.Application.Common.Interfaces;

namespace StoreOnline.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("CleanArchitecture Request: {Name} {@Request}",
            requestName, request);
        return Task.CompletedTask;
    }
}
