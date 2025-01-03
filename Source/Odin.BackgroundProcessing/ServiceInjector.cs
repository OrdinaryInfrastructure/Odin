using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
        /// Sets up IBackgroundProcessor in DI from configuration
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName">Cryptography by default</param>
        public static void AddBackgroundProcessing(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            string sectionName = "BackgroundProcessing")
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
            serviceCollection.AddBackgroundProcessing(configuration, section);
        }

        /// <summary>
        /// Sets up EmailSending from the provided ConfigurationSection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static void AddBackgroundProcessing(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            IConfigurationSection configurationSection)
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
                    configurationSection);
            }
            else
            {
                string message = $"Unable to load provider {providerName} from {providerAssemblyName}.";
                message += $"This can occur if the {providerAssemblyName} Nuget package reference is missing. {serviceInjectorCreation.MessagesToString()}";

                throw new ApplicationException(message);
            }
        }

        /// <summary>
        /// Sets up Background Processing
        /// </summary>
        /// <param name="appServices"></param>
        /// <param name="app"></param>
        public static IHost UseBackgroundProcessing(this IHost app, IServiceProvider appServices)
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