using AsyncFlow.Core;
using AsyncFlow.Core.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncFlow.ServiceCollection
{
    /// <summary>
    /// Provides extension methods to configure AsyncFlow within an IServiceCollection.
    /// </summary>
    public static class AsyncFlowServiceExtensions
    {
        /// <summary>
        /// Adds and configures services for AsyncFlow.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="setupAction">A delegate to configure the provided <see cref="AsyncFlowOptions"/>.</param>
        /// <returns>The original IServiceCollection to allow for chaining.</returns>
        public static IServiceCollection AddAsyncFlow(this IServiceCollection services, Action<AsyncFlowOptions> setupAction)
        {
            var options = new AsyncFlowOptions();
            setupAction(options);

            services.AddSingleton(options.Cache);
            
            return services;
        }

        /// <summary>
        /// Configures the provided <see cref="AsyncFlowOptions"/> to use in-memory caching.
        /// </summary>
        /// <param name="options">The options to configure.</param>
        /// <param name="memoryCacheOptions">Optional configurations for the memory cache. If not provided, default configurations will be used.</param>
        /// <returns>The configured <see cref="AsyncFlowOptions"/>.</returns>
        public static AsyncFlowOptions UseMemoryCache(this AsyncFlowOptions options, MemoryCacheOptions? memoryCacheOptions = null)
        {
            memoryCacheOptions ??= new MemoryCacheOptions();
            options.Cache = new MemoryFlowCache(new MemoryCache(memoryCacheOptions));
            return options;
        }

        /// <summary>
        /// Configures the provided <see cref="AsyncFlowOptions"/> to use distributed caching.
        /// </summary>
        /// <param name="options">The options to configure.</param>
        /// <param name="cache">The instance of IDistributedCache to be used for caching.</param>
        /// <returns>The configured <see cref="AsyncFlowOptions"/>.</returns>
        public static AsyncFlowOptions UseDistributedCache(this AsyncFlowOptions options, IDistributedCache cache)
        {
            options.Cache = new DistributedFlowCache(cache);
            return options;
        }
    }
}
