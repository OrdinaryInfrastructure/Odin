using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Odin.BackgroundProcessing
{
    public interface IBackgroundProcessorServiceInjector
    {
        /// <summary>
        /// Adds BackgroundProcessing services, such as Hangfire's server.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configSection"></param>
        void TryAddBackgroundProcessor(IServiceCollection serviceCollection, IConfiguration configuration, IConfigurationSection configSection, string? sqlServerConnectionString = null);

        /// <summary>
        /// Adds BackgroundProcessing features (such as Hangfire's dashboard) to the HTTP pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appServices"></param>
        /// <returns></returns>
        IApplicationBuilder UseBackgroundProcessing(IApplicationBuilder app, IServiceProvider appServices);
        
    }
}