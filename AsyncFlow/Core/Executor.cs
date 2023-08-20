using AsyncFlow.Core.Cache;
using AsyncFlow.Helpers;
using AsyncFlow.Interfaces;
using AsyncFlow.Responses;
using Hangfire.Server;
using Microsoft.Extensions.Caching.Memory;

namespace AsyncFlow.Core;

internal class Executor<TFlow,TRequest,TResult> where TFlow: IAsyncFlow<TRequest,TResult>
{
    private readonly TFlow _flow;
    private readonly IAsyncFlowCache _asyncFlowCache;

    public Executor(TFlow flow, IAsyncFlowCache asyncFlowCache)
    {
        _flow = flow;
        _asyncFlowCache = asyncFlowCache;
    }

    public async Task ExecuteAsync(TRequest request, PerformContext? context,CancellationToken cancellationToken=default)
    {
        var progress=new DelegateProgress<ProgressData>(data=>context!.SetJobParameter("Progress",data));
        var result= await _flow.ProcessAsync(request,progress,cancellationToken);
        _asyncFlowCache.Set(context!.BackgroundJob.Id, result);
    }
}