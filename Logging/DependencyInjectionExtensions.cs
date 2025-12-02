using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to support ILogger2 of T 
    /// </summary>
    public static class Logger2Extensions
    {
        /// <summary>
        /// Sets up ILogger2 of T in dependency injection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static void AddLogger2(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton(typeof(ILogger2<>), typeof(Logger2<>));
        }
    }
}