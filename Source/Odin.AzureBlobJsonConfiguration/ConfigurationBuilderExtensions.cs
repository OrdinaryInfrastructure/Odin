using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Odin.AzureBlobJsonConfiguration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddAzureBlobJsonConfiguration(this IConfigurationBuilder configBuilder, BlobJsonConfigurationOptions options, Action<FileLoadExceptionContext>? onLoadException)
    {
        return configBuilder.Add(new BlobJsonConfigurationSource(options, onLoadException));
    }
    
    public static IConfigurationBuilder AddAzureBlobJsonConfiguration(this IConfigurationBuilder configBuilder, BlobJsonConfigurationOptions options, TokenCredential tokenCredential, Action<FileLoadExceptionContext>? onLoadException)
    {
        return configBuilder.Add(new BlobJsonConfigurationSource(options, tokenCredential, onLoadException));
    }
    
    public static IConfigurationBuilder AddAzureBlobJsonConfiguration(this IConfigurationBuilder configBuilder, BlobJsonConfigurationOptions options, BlobClient blobClient, Action<FileLoadExceptionContext>? onLoadException)
    {
        return configBuilder.Add(new BlobJsonConfigurationSource(options, blobClient, onLoadException));
    }
}