using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to support IMockableLogger of T 
    /// </summary>
    public static class MockableLoggerExtensions
    {
        /// <summary>
        /// Sets up IMockableLogger of T
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static void AddMockableLogger(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton(typeof(IMockableLogger<>), typeof(MockableLogger<>));
        }
    }
}