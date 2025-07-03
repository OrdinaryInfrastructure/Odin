using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Odin.AzureBlobJsonConfiguration;

public class BlobJsonConfigurationSource : JsonConfigurationSource
{
    public BlobJsonConfigurationSource(BlobJsonConfigurationOptions options, Action<FileLoadExceptionContext>? onLoadException)
    {
        Options = options;
        OnLoadException = onLoadException;
    }

    public BlobJsonConfigurationSource(BlobJsonConfigurationOptions options, TokenCredential credential, Action<FileLoadExceptionContext>? onLoadException)
    {
        Options = options;
        TokenCredentialOverride = credential;
        OnLoadException = onLoadException;
    }

    public BlobJsonConfigurationSource(BlobJsonConfigurationOptions options, BlobClient blobClient, Action<FileLoadExceptionContext>? onLoadException)
    {
        Options = options;
        BlobClientOverride = blobClient;
        OnLoadException = onLoadException;
    }

    internal BlobJsonConfigurationOptions Options { get; }

    internal TokenCredential? TokenCredentialOverride { get; }

    internal BlobClient? BlobClientOverride { get; }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new BlobJsonConfigurationProvider(this);
    }

    internal BlobClient GetBlobClient()
    {
        if (BlobClientOverride is not null)
        {
            return BlobClientOverride;
        }

        if (Options.BlobUri is null)
        {
            throw new ApplicationException("Cannot construct BlobClient: No BlobUri configured.");
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