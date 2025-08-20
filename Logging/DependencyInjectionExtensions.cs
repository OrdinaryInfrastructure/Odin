using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to support ILoggerAdapter of T 
    /// </summary>
    public static class LoggerAdapterExtensions
    {
        /// <summary>
        /// Sets up ILoggerAdapter of T in dependency injection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static void AddLoggerAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        }
    }
}