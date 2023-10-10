using Hangfire.Server;

namespace AsyncFlow.Core;

public interface IExecutor< TRequest> 
{
    Task ExecuteAsync(string name, TRequest request, PerformContext? context,
        CancellationToken cancellationToken = default);
}