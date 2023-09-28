using AsyncFlow.Configuration;
using AsyncFlow.Core;
using AsyncFlow.Core.Cache;
using AsyncFlow.Extensions;
using AsyncFlow.Helpers;
using AsyncFlow.Interfaces;
using AsyncFlow.Responses;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncFlow;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps the asynchronous flow endpoints to the specified web application for processing requests, checking status, and retrieving results.
    /// </summary>
    /// <typeparam name="TFlow">The type of the asynchronous flow that implements <see cref="IAsyncFlow{TRequest, TResponse}"/>.</typeparam>
    /// <typeparam name="TRequest">The type of the input request.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned after processing the request.</typeparam>
    /// <param name="app">The web application to which the flow endpoints will be mapped.</param>
    /// <param name="flowName">The name of the flow used to define the route segment for the flow's endpoints.</param>
    /// <param name="configurator">An instance of <see cref="AsyncFlowEndpointConfigurator"/> which allows users to configure the behavior of the flow endpoints. If not provided, default behaviors are used.</param>
    /// <returns>The same web application after mapping the flow's endpoints, allowing for further configuration or endpoint mapping.</returns>
    public static WebApplication MapFlow<TFlow, TRequest, TResponse>(this WebApplication app, string flowName, AsyncFlowEndpointConfigurator? configurator=default) 
        where TFlow : IAsyncFlow<TRequest, TResponse>
    {
        var enqueueEndpoint=app.MapPost($"/{flowName}", HandleEnqueue<TFlow, TRequest, TResponse>);
        configurator?.EnqueueConfiguration?.Invoke(enqueueEndpoint);

        app.MapGet($"/{flowName}/{{jobId}}/status", HandleGetStatus);
        configurator?.StatusConfiguration?.Invoke(enqueueEndpoint);

        app.MapGet($"/{flowName}/{{jobId}}/error", HandleGetError);
        configurator?.ErrorConfiguration?.Invoke(enqueueEndpoint);
        
        app.MapGet($"/{flowName}/{{jobId}}/result", HandleGetResult<TResponse>);
        configurator?.ResultConfiguration?.Invoke(enqueueEndpoint);

        app.MapDelete($"/{flowName}/{{jobId}}", HandleDeleteResult);
        configurator?.DeleteConfiguration?.Invoke(enqueueEndpoint);

        return app;
    }

    private static Task HandleDeleteResult(HttpContext context,string jobId)
    {
        BackgroundJob.Delete(jobId);
        var cache = context.RequestServices.GetRequiredService<IAsyncFlowCache>();
        cache.Delete(jobId);
        return Task.CompletedTask;
    }


    private static Task<IResult> HandleGetResult<TResponse>(HttpContext context, string jobId)
    {
        var cache = context.RequestServices.GetRequiredService<IAsyncFlowCache>();
        var result = cache.Get<TResponse>(jobId);
        return Task.FromResult(result == null ? Results.NotFound("Resource not found") : Results.Ok(result));
    }

    private static Task<StatusResponse> HandleGetStatus(HttpContext context,string jobId)
    {
        var connection = JobStorage.Current.GetConnection();
        var jobData = connection.GetJobData(jobId);
        var stateName = jobData.State;
        var statusResponse = new StatusResponse(jobId,stateName,jobData.CreatedAt);
        
        
        var progressData = connection.GetJobParameter(jobId,"Progress");
        if(!string.IsNullOrEmpty(progressData))
            statusResponse=statusResponse with { ProgressData = SerializationHelper.Deserialize<ProgressData>(progressData) };
       
        return Task.FromResult<StatusResponse>(statusResponse);
    }
    private static Task<IResult> HandleGetError(HttpContext context,string jobId)
    {
        var connection = JobStorage.Current.GetConnection();
        var jobData = connection.GetJobData(jobId);
        var stateName = jobData.State;
        switch (stateName)
        {
            case "Failed":
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var failureDto = monitoringApi.GetFailedJobs().FirstOrDefault(job => job.Key == jobId);


                var error = new Error(failureDto.Value.ExceptionType,failureDto.Value.ExceptionMessage,failureDto.Value.ExceptionDetails);
                return Task.FromResult<IResult>(Results.Ok(new ErrorResponse(jobId,error)));
            }
            default:
                return Task.FromResult(Results.NoContent());
        }
    }

    private static Task<EnqueueResponse> HandleEnqueue<TFlow,TRequest,TResponse>(TRequest request)where TFlow : IAsyncFlow<TRequest,TResponse>
    {
        var jobId = BackgroundJob.Enqueue<Executor<TFlow,TRequest,TResponse>>(typeof(TFlow).GetQueueName(),flowExecutor => flowExecutor.ExecuteAsync(typeof(TFlow).Name,request,null,CancellationToken.None));
        return Task.FromResult(new EnqueueResponse(jobId, DateTime.Now));
    }
}
