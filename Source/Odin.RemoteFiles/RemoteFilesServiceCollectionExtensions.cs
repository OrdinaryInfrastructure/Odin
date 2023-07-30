using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.DesignContracts;

namespace Odin.RemoteFiles;

public static class RemoteFilesServiceCollectionExtensions
{
    public static IServiceCollection AddRemoteFiles(this IServiceCollection services, IConfiguration configuration,
        string sectionName = RemoteFilesOptions.RemoteFilesConfigurationPosition)
    {
        return AddRemoteFiles(services, configuration.GetSection(sectionName));
    }

    public static IServiceCollection AddRemoteFiles(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        PreCondition.RequiresNotNull(configurationSection, "Configuration Section for RemoteFiles cannot be null.");
        
        if (!configurationSection.Exists())
            throw new ApplicationException(
                $"Configuration section '{configurationSection.Key}' missing. Cannot configure remote files.");

        RemoteFilesOptions remoteFilesOptions = new RemoteFilesOptions();
        configurationSection.Bind(remoteFilesOptions);

        services.TryAddSingleton(remoteFilesOptions);
        services.TryAddSingleton<IRemoteFileSessionFactory, RemoteFileSessionFactory>();

        return services;
    }
}