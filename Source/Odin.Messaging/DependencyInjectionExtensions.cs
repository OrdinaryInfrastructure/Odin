using System;
using System.Collections.Generic;
using System.Linq;
using Odin.Messaging;
using Odin.Messaging.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to support messaging services setup from configuration
    /// </summary>
    public static class MessagingExtensions
    {
        /// <summary>
        /// Sets up IBackgroundProcessor in DI from configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName">Cryptography by default</param>
        public static void AddMessaging(
            this IServiceCollection services, IConfiguration configuration,
            string sectionName = "Messaging")
        {
            MessagingSettings settings = new MessagingSettings();
            IConfigurationSection messagingConfig = configuration.GetSection(sectionName);
            messagingConfig.Bind(settings);
            Outcome configOutcome = settings.IsConfigurationValid();
            if (!configOutcome.Success)
            {
                throw new ApplicationException($"{nameof(AddMessaging)}: Invalid MessagingSettings in section {sectionName}. Errors are: {configOutcome.MessagesToString()}");
            }
            if (settings.Provider == Providers.RabbitMQ)
            {
                List<IConfigurationSection> sections = messagingConfig.GetChildren().ToList();
                // Todo: improve on finding section using Linq?
                IConfigurationSection rabbitConfig = sections.FirstOrDefault(configuration => configuration.Key == Providers.RabbitMQ);
                if (rabbitConfig==null)
                {
                    throw new ApplicationException($"{nameof(AddMessaging)}: {Providers.RabbitMQ} section missing in section {sectionName}.");
                }
                RabbitSettings rabbitSettings = new RabbitSettings();
                rabbitConfig.Bind(rabbitSettings);
                Outcome settingsValid = rabbitSettings.IsConfigurationValid();
                if (!settingsValid.Success)
                {
                    throw new ApplicationException($"{nameof(AddMessaging)}: Invalid {Providers.RabbitMQ} settings in section {sectionName}. Errors are: {settingsValid.MessagesToString()}");
                }
                
                // Add RabbitMQ settings as a singleton, which is needed by RabbitMessagingProducer
                services.TryAddSingleton(rabbitSettings);

                // Run with singleton producer for 1 long running Rabbit connection.
                services.TryAddSingleton<IMessagingProducer, RabbitMessagingProducer>();
            }
            else if (settings.Provider == Providers.FakeMessaging)
            {
                services.TryAddSingleton<IMessagingProducer, FakeMessagingProducer>();
            }
            else
            {
                throw new NotImplementedException($"MessagingSettings Provider {settings.Provider} is not implemented.");
            }
        }
    }
}