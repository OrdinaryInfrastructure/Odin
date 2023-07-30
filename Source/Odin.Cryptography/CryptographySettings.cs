using System.Collections.Generic;
using System.Linq;
using Odin.System;


namespace Odin.Cryptography
{
    /// <summary>
    /// Cryptography settings 
    /// </summary>
    public sealed class CryptographySettings
    {
        /// <summary>
        /// FakeCryptographer or DataProtectionCryptographer
        /// </summary>
        public string Provider { get; set; } 
        
        /// <summary>
        /// Name of the application 
        /// </summary>
        public string ApplicationName { get; set; } 
        
        /// <summary>
        /// If set, the location on disk to store DPAPI Keys...
        /// </summary>
        public string PersistKeysToDirectory { get; set; } 

        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Outcome IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Provider))
            {
                errors.Add("Provider is missing");
            }
            else if (!Providers.GetSupportedProviders().Contains(Provider))
            {
                errors.Add($"The {nameof(Provider)} configured ({Provider}) is not one of the supported providers: {string.Join(" | ",Providers.GetSupportedProviders())}");
            }
            else if (Provider == Providers.DataProtectionCryptographer && string.IsNullOrWhiteSpace(ApplicationName))
            {
                errors.Add("ApplicationName is required for DataProtectionCryptographer");
            }
            return new Outcome(!errors.Any(), errors);
        }
    }
}
