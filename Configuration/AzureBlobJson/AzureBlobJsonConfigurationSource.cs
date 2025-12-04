using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Odin.Configuration;


public class AzureBlobJsonConfigurationSource : JsonConfigurationSource
{
    public AzureBlobJsonConfigurationSource(AzureBlobJsonConfigurationOptions options, Action<FileLoadExceptionContext>? onLoadException)
    {
        Options = options;
        OnLoadException = onLoadException;
    }

    public AzureBlobJsonConfigurationSource(AzureBlobJsonConfigurationOptions options, TokenCredential credential, Action<FileLoadExceptionContext>? onLoadException)
    {
        Options = options;
        TokenCredentialOverride = credential;
        OnLoadException = onLoadException;
    }

    public AzureBlobJsonConfigurationSource(AzureBlobJsonConfigurationOptions options, BlobClient blobClient, Action<FileLoadExceptionContext>? onLoadException)
    {
        Options = options;
        BlobClientOverride = blobClient;
        OnLoadException = onLoadException;
    }

    internal AzureBlobJsonConfigurationOptions Options { get; }

    internal TokenCredential? TokenCredentialOverride { get; }

    internal BlobClient? BlobClientOverride { get; }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AzureBlobJsonConfigurationProvider(this);
    }

    internal BlobClient GetBlobClient()
    {
        if (BlobClientOverride is not null)
        {
            return BlobClientOverride;
        }

        if (Options.BlobUri is null)
        {
            throw new ApplicationException("Cannot construct BlobClient. No BlobUri configured.");
        }

        if (TokenCredentialOverride is not null)
        {
            return new BlobClient(Options.BlobUri, TokenCredentialOverride);
        }

        if (Options.SharedKeyCredential is null)
        {
            throw new ApplicationException($"Cannot construct BlobClient: No TokenCredential override provided, and SharedKeyCredential missing from configuration.");
        }

        return new BlobClient(Options.BlobUri, Options.SharedKeyCredential.GetCredential());
    }
}