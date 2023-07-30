using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// Sends an email
    /// </summary>
    public interface IBackgroundProcessorServiceInjector
    {
        /// <summary>
        /// Supports injection of specific EmailSender
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configSection"></param>
        void TryAddBackgroundProcessor(IServiceCollection serviceCollection, IConfiguration configuration, IConfigurationSection configSection);

        /// <summary>
        /// Inserts background processing middleware using IApplicationBuilder
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="appServices"></param>
        /// <returns></returns>
        IApplicationBuilder UseBackgroundProcessing(IApplicationBuilder appBuilder, IServiceProvider appServices);
    }
}