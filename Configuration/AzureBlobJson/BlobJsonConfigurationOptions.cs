using System.ComponentModel.DataAnnotations;
using Azure.Storage;

namespace Odin.Configuration.AzureBlobJson;

public class BlobJsonConfigurationOptions
{
    public bool IsDisabled { get; set; }
    
    public double ReloadPeriodSeconds { get; set; } = 120;

    /// <summary>
    /// e.g. "https://outgoingpaymentsstorage.blob.core.windows.net/dynamic-config/dynamic-config-LocalDev.json"
    /// </summary>
    [Required]
    public Uri BlobUri { get; set; } = null!;
    
    public SharedKeyCredentialOptions? SharedKeyCredential { get; set; }
}

public class SharedKeyCredentialOptions
{
    public string AccountName { get; set; } = null!;
    public string AccountKey { get; set; } = null!;

    public StorageSharedKeyCredential GetCredential()
    {
        return new StorageSharedKeyCredential(AccountName, AccountKey);
    }
}



