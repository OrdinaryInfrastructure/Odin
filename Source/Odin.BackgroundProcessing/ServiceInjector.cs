using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;
using Odin.System;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// Dependency injection methods to support Background Processing services setup by adding an IBackgroundProcessor
    /// from configuration
    /// </summary>
    public static class ServiceInjector
    {
        /// <summary>
        /// Adds BackgroundProcessing services (such as Hangfire's server) according to the provided ConfigurationSection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName">Cryptography by default</param>
        /// <param name="sqlServerConnectionString"></param>
        public static void AddBackgroundProcessing(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            string sectionName = "BackgroundProcessing", string? sqlServerConnectionString = null)
        {
            IConfigurationSection? section = configuration.GetSection(sectionName);
            if (section == null)
            {
                section = configuration.GetSection($"{sectionName}BackgroundProcessor");
                if (section == null)
                {
                    throw new ApplicationException(
                        $"{nameof(AddBackgroundProcessing)}: Section {sectionName} missing in configuration.");
                }
            }
            serviceCollection.AddBackgroundProcessing(configuration, section, sqlServerConnectionString);
        }
        
        /// <summary>
        /// Adds BackgroundProcessing services
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <param name="sqlServerConnectionStringFactory"></param>
        /// <exception cref="ApplicationException"></exception>
        public static void AddBackgroundProcessing(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            string sectionName, Func<IServiceProvider, string>? sqlServerConnectionStringFactory = null)
        {
            IConfigurationSection? section = configuration.GetSection(sectionName);
            if (section == null)
            {
                section = configuration.GetSection($"{sectionName}BackgroundProcessor");
                if (section == null)
                {
                    throw new ApplicationException(
                        $"{nameof(AddBackgroundProcessing)}: Section {sectionName} missing in configuration.");
                }
            }
            serviceCollection.AddBackgroundProcessing(configuration, section, sqlServerConnectionStringFactory);
        }

        /// <summary>
        /// Adds BackgroundProcessing services
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="configurationSection"></param>
        /// <param name="sqlServerConnectionString"></param>
        public static void AddBackgroundProcessing(this IServiceCollection serviceCollection, IConfiguration configuration, IConfigurationSection configurationSection,
            string? sqlServerConnectionString = null)
        {
            AddBackgroundProcessing(serviceCollection, configuration, configurationSection, sqlServerConnectionString is null ? null : _ => sqlServerConnectionString);
        }

        /// <summary>
        /// Adds BackgroundProcessing services (such as Hangfire's server) according to the provided ConfigurationSection 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="configurationSection"></param>
        /// <param name="sqlServerConnectionStringFactory"></param>
        /// <returns></returns>
        public static void AddBackgroundProcessing(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            IConfigurationSection configurationSection, Func<IServiceProvider, string>? sqlServerConnectionStringFactory = null)
        {
            PreCondition.RequiresNotNull(configurationSection);

            BackgroundProcessingOptions options = new BackgroundProcessingOptions();
            configurationSection.Bind(options);

            Outcome configOutcome = options.Validate();
            if (!configOutcome.Success)
            {
                throw new ApplicationException(
                    $"{nameof(AddBackgroundProcessing)}: Invalid BackgroundProcessing configuration. Errors are: {configOutcome.MessagesToString()}");
            }

            serviceCollection.TryAddSingleton(options);


            if (options.Provider == BackgroundProcessingProviders.Fake)
            {
                serviceCollection.AddTransient<IBackgroundProcessor, FakeBackgroundProcessor>();
                return;
            }
            
            // Convention currently is that Providers are always located in an assembly called Odin.BackgroundProcessing.ProviderName
            // If not we will can AssemblyName into the config.
            string providerAssemblyName = $"Odin.BackgroundProcessing.{options.Provider}";
            string providerName = $"Odin.BackgroundProcessing.{options.Provider}BackgroundProcessor";

            ClassFactory activator = new ClassFactory();
            Outcome<IBackgroundProcessorServiceInjector> serviceInjectorCreation =
                activator.TryCreate<IBackgroundProcessorServiceInjector>(
                    $"{providerAssemblyName}ServiceInjector", providerAssemblyName);

            if (serviceInjectorCreation.Success)
            {
                serviceInjectorCreation.Value.TryAddBackgroundProcessor(serviceCollection, configuration,
                    configurationSection, sqlServerConnectionStringFactory);
            }
            else
            {
                string message = $"Unable to load provider {providerName} from {providerAssemblyName}.";
                message += $"This can occur if the {providerAssemblyName} Nuget package reference is missing. {serviceInjectorCreation.MessagesToString()}";

                throw new ApplicationException(message);
            }
        }

        /// <summary>
        /// Adds BackgroundProcessing features (such as Hangfire's dashboard) to the HTTP pipeline.
        /// </summary>
        /// <param name="appServices"></param>
        /// <param name="app"></param>
        public static IApplicationBuilder UseBackgroundProcessing(this IApplicationBuilder app, IServiceProvider appServices)
        {
            PreCondition.RequiresNotNull(appServices);
            PreCondition.RequiresNotNull(app);

            BackgroundProcessingOptions options = appServices.GetRequiredService<BackgroundProcessingOptions>();
            if (options.Provider == BackgroundProcessingProviders.Fake)
            {
                return app;
            }

            ClassFactory activator = new ClassFactory();
            string providerAssemblyName = $"Odin.BackgroundProcessing.{options.Provider}";
            Outcome<IBackgroundProcessorServiceInjector> serviceInjectorCreation =
                activator.TryCreate<IBackgroundProcessorServiceInjector>(
                    $"{providerAssemblyName}ServiceInjector", providerAssemblyName);

            if (serviceInjectorCreation.Success)
            {
                return serviceInjectorCreation.Value.UseBackgroundProcessing(app, appServices);
            }

            string message = $"Unable to load provider Odin.BackgroundProcessing.{options.Provider}.";
            if (BackgroundProcessingProviders.IsBuiltInProvider(options.Provider))
            {
                message += $"This can occur if the {options.Provider} Nuget package reference is missing.";
            }
            else
            {
                message += $"{options.Provider} is not a recognised IBackgroundProcessor provider.";
            }

            throw new ApplicationException(message);
        }
    }
}