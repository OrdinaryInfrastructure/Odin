using System.Collections.Generic;
using System.Linq;
using Odin.System;


namespace Odin.Messaging
{
    /// <summary>
    /// MessagingSettings for loading from configuration 
    /// </summary>
    public sealed class MessagingSettings
    {
        /// <summary>
        /// FakeMessagingServices or RabbitMQMessagingServices
        /// </summary>
        public string Provider { get; set; } = Providers.FakeMessaging;

        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Outcome IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            List<string> providers = Providers.GetSupportedProviders();
            if (string.IsNullOrWhiteSpace(Provider))
            {
                errors.Add($"{nameof(Provider)} has not been specified. Must be 1 of {string.Join(" | ",providers)}");
            }
            else if (!providers.Contains(Provider))
            {
                errors.Add($"The {nameof(Provider)} specified ({Provider}) is not one of the supported providers: {string.Join(" | ",providers)}");
            }
            return new Outcome(!errors.Any(), errors);
        }
    }
}
