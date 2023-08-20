using Microsoft.AspNetCore.Builder;

namespace AsyncFlow.Configuration;

/// <summary>
/// Provides an abstract base for configuring the behavior of async flow endpoints.
/// This allows custom configuration for endpoints related to enqueuing, status retrieval, result retrieval, and result deletion.
/// </summary>
public abstract class AsyncFlowEndpointConfigurator
{
    internal Action<RouteHandlerBuilder>? EnqueueConfiguration;
    internal Action<RouteHandlerBuilder>? StatusConfiguration;
    internal Action<RouteHandlerBuilder>? ResultConfiguration;
    internal Action<RouteHandlerBuilder>? DeleteConfiguration;
    internal Action<RouteHandlerBuilder>? ErrorConfiguration;

    /// <summary>
    /// Configures the behavior of the enqueue endpoint.
    /// </summary>
    /// <param name="configuration">The action to configure the enqueue endpoint.</param>
    /// <returns>The current instance of <see cref="AsyncFlowEndpointConfigurator"/> for further configuration.</returns>
    public AsyncFlowEndpointConfigurator ForEnqueueEndpoint(Action<RouteHandlerBuilder> configuration)
    {
        EnqueueConfiguration = configuration;
        return this;
    }

    /// <summary>
    /// Configures the behavior of the status retrieval endpoint.
    /// </summary>
    /// <param name="configuration">The action to configure the status retrieval endpoint.</param>
    /// <returns>The current instance of <see cref="AsyncFlowEndpointConfigurator"/> for further configuration.</returns>
    public AsyncFlowEndpointConfigurator ForStatusEndpoint(Action<RouteHandlerBuilder> configuration)
    {
        StatusConfiguration = configuration;
        return this;
    }

    
    /// <summary>
    /// Configures the behavior of the error retrieval endpoint.
    /// </summary>
    /// <param name="configuration">The action to configure the error retrieval endpoint.</param>
    /// <returns>The current instance of <see cref="AsyncFlowEndpointConfigurator"/> for further configuration.</returns>
    public AsyncFlowEndpointConfigurator ForErrorEndpoint(Action<RouteHandlerBuilder> configuration)
    {
        StatusConfiguration = configuration;
        return this;
    }
    
    /// <summary>
    /// Configures the behavior of the result retrieval endpoint.
    /// </summary>
    /// <param name="configuration">The action to configure the result retrieval endpoint.</param>
    /// <returns>The current instance of <see cref="AsyncFlowEndpointConfigurator"/> for further configuration.</returns>
    public AsyncFlowEndpointConfigurator ForResultEndpoint(Action<RouteHandlerBuilder> configuration)
    {
        ResultConfiguration = configuration;
        return this;
    }

    /// <summary>
    /// Configures the behavior of the result deletion endpoint.
    /// </summary>
    /// <param name="configuration">The action to configure the result deletion endpoint.</param>
    /// <returns>The current instance of <see cref="AsyncFlowEndpointConfigurator"/> for further configuration.</returns>
    public AsyncFlowEndpointConfigurator ForDeleteEndpoint(Action<RouteHandlerBuilder> configuration)
    {
        DeleteConfiguration = configuration;
        return this;
    }
}

