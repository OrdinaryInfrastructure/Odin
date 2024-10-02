namespace Odin.Email.Office365;

public record Office365Options
{
    public MicrosoftGraphClientSecretCredentials? MicrosoftGraphClientSecretCredentials { get; set; }
    
    public record Sender
    {
        /// <summary>
        /// Microsoft UserId (i.e. email address) as whom the app has permissions to send mail.
        /// Sender will send email as this userId unless IEmailMessage.From is specified. 
        /// </summary>
        public required string SenderUserId { get; set; }
        
        /// <summary>
        /// Office365 Categories which will be added to each email sent by this sender.
        /// </summary>
        public List<string>? DefaultCategories { get; set; }
    }
    
    /// <summary>
    /// For each key, adds a KeyedSingleton IEmailSender in DI.
    /// </summary>
    public Dictionary<string, Sender> KeyedSenders { get; set; } = [];

    public void Validate()
    {
        if (MicrosoftGraphClientSecretCredentials is null)
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
        
        foreach (var sender in KeyedSenders.Values)
        {
            if (string.IsNullOrWhiteSpace(sender.SenderUserId))
            {
                throw new ApplicationException("SenderUserId null or whitespace");
            }
        }
    }
};