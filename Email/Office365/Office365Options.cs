namespace Odin.Email;

public record Office365Options
{
    public MicrosoftGraphClientSecretCredentials MicrosoftGraphClientSecretCredentials { get; set; } = null!;

    /// <summary>
    /// Microsoft UserId (i.e. email address) as whom the app has permissions to send mail.
    /// Sender will send email as this userId if specified. If not specified then EMailOptions.DefaultFromAddress (not IEmailMessage.From) is used
    /// as the SenderUserId. 
    /// </summary>
    public string? SenderUserId { get; set; }

    public void Validate()
    {
        if (MicrosoftGraphClientSecretCredentials == null)
        {
            throw new ApplicationException($"{nameof(MicrosoftGraphClientSecretCredentials)} null");
        }
        if (string.IsNullOrWhiteSpace(MicrosoftGraphClientSecretCredentials.ClientId))
        {
            throw new ApplicationException($"{nameof(MicrosoftGraphClientSecretCredentials.ClientId)} null or whitespace");
        }
        if (string.IsNullOrWhiteSpace(MicrosoftGraphClientSecretCredentials.TenantId))
        {
            throw new ApplicationException($"{nameof(MicrosoftGraphClientSecretCredentials.TenantId)} null or whitespace");
        }
        if (string.IsNullOrWhiteSpace(MicrosoftGraphClientSecretCredentials.ClientSecret))
        {
            throw new ApplicationException($"{nameof(MicrosoftGraphClientSecretCredentials.ClientSecret)} null or whitespace");
        }
    }
};