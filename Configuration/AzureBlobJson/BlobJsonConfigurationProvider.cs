using System.Runtime.ExceptionServices;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Odin.Configuration.AzureBlobJson;

public class BlobJsonConfigurationProvider : JsonConfigurationProvider
{
    private Timer _timer;
    public BlobJsonConfigurationProvider(BlobJsonConfigurationSource source) : base(source)
    {
        TimeSpan period = TimeSpan.FromSeconds(source.Options.ReloadPeriodSeconds);
        _timer = new Timer(AutoReload, null, period, period);
    }

    private BlobClient GetBlobClient()
    {
        if (Source is not BlobJsonConfigurationSource blobSource)
        {
            throw new ApplicationException($"{nameof(BlobJsonConfigurationProvider)}.{nameof(Source)} is {Source.GetType()}, not {nameof(BlobJsonConfigurationSource)}");
        }

        return blobSource.GetBlobClient();
    }

    private int _isAutoReloading = 0;

    private void AutoReload(object? _)
    {
        if (Interlocked.Exchange(ref _isAutoReloading, 1) == 1)
        {
            return;
        }

        try
        {
            Load();
        }
        finally
        {
            Interlocked.Exchange(ref _isAutoReloading, 0);
        }
    }

    public override void Load()
    {
        try
        {
            LoadAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            HandleException(ExceptionDispatchInfo.Capture(ex));
        }
    }

    private readonly SemaphoreSlim _loadAsyncSemaphore = new(1);

    private ETag? _mostRecentEtag;

    private async Task LoadAsync()
    {
        await _loadAsyncSemaphore.WaitAsync();
        try
        {
            BlobClient client = GetBlobClient();
            
            var attributes = await client.GetPropertiesAsync();

            if (attributes.HasValue && attributes.Value.ETag == _mostRecentEtag)
            {
                return;
            }

            using MemoryStream memStream = new MemoryStream();

            await client.DownloadToAsync(memStream);

            memStream.Position = 0;

            Load(memStream);

            _mostRecentEtag = attributes.HasValue ? attributes.Value.ETag : null;

            OnReload();
        }
        finally
        {
            _loadAsyncSemaphore.Release();
        }
    }

    // Copied from Microsoft.Extensions.Configuration.FileConfigurationProvider
    private void HandleException(ExceptionDispatchInfo info)
    {
        bool ignoreException = false;
        if (Source.OnLoadException != null)
        {
            FileLoadExceptionContext exceptionContext = new FileLoadExceptionContext
            {
                Provider = this,
                Exception = info.SourceException
            };
            Source.OnLoadException.Invoke(exceptionContext);
            ignoreException = exceptionContext.Ignore;
        }

        if (!ignoreException)
        {
            info.Throw();
        }
    }
}