using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Core.Application.Behaviors.Performance;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>

    where TRequest : IRequest<TResponse>, IPerformanceRequest
{

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();

        TResponse response = await next();

        stopWatch.Stop();

        if (stopWatch.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning($"{request.GetType()} : {stopWatch.ElapsedMilliseconds}");
        }

        return response;

    }
}
