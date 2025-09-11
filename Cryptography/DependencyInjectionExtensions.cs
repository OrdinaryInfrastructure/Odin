using Odin.Cryptography;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Odin.System;


namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to support Background Processing services setup by adding an IBackgroundProcessor
    /// from configuration
    /// </summary>
    public static class CryptographyExtensions
    {
        /// <summary>
        /// Sets up IBackgroundProcessor in DI from configuration
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName">Cryptography by default</param>
        public static void AddCryptography(
            this IServiceCollection serviceCollection, IConfiguration configuration,
            string sectionName = "Cryptography")
        {
            CryptographySettings settings = new CryptographySettings();
            configuration.Bind(sectionName, settings);

            Result configValidation = settings.IsConfigurationValid();

            if (!configValidation.Success)
            {
                throw new ApplicationException($"Invalid CryptographySettings in section {sectionName}. Errors are: {configValidation.MessagesToString()}");
            }

            if (settings.Provider == Providers.FakeCryptographer)
            {
                serviceCollection.AddSingleton<ICryptographer, FakeCryptographer>();
            }
            else if (settings.Provider == Providers.DataProtectionCryptographer)
            {
                IDataProtectionBuilder builder = serviceCollection.AddDataProtection();
                if (!string.IsNullOrWhiteSpace(settings.ApplicationName))
                {
                    builder = builder.SetApplicationName(settings.ApplicationName);
                }

                if (!string.IsNullOrWhiteSpace(settings.PersistKeysToDirectory))
                {
                    builder.PersistKeysToFileSystem(
                        new DirectoryInfo(settings.PersistKeysToDirectory));
                }

                serviceCollection.AddSingleton<ICryptographer, DataProtectionCryptographer>();
            }
        }
    }
}