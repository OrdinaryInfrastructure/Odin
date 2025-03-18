using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Odin.BackgroundProcessing
{
    public interface IBackgroundProcessorServiceInjector
    {
        /// <summary>
        /// Supports injection of specific EmailSender
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configSection"></param>
        void TryAddBackgroundProcessor(IServiceCollection serviceCollection, IConfiguration configuration, IConfigurationSection configSection, string? sqlServerConnectionString = null);

        /// <summary>
        /// Inserts background processing middleware using IHost / IApplicationBuilder
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appServices"></param>
        /// <returns></returns>
        IHost UseBackgroundProcessing(IHost app, IServiceProvider appServices);
    }
}