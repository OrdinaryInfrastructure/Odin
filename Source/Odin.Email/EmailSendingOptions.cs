using System;
using System.Collections.Generic;
using System.Linq;
using Odin.DesignContracts;
using Odin.System;


namespace Odin.Email
{
    /// <summary>
    /// Email settings for loading from configuration 
    /// </summary>
    public sealed record EmailSendingOptions
    {
        private string _provider = EmailSendingProviders.Mailgun;

        /// <summary>
        /// Name of the default EmailSending configuration section in application IConfiguration
        /// </summary>
        public const string DefaultConfigurationSectionName = "EmailSending";
        
        /// <summary>
        /// Default from emailAddress for the generic IEmailSender in DI
        /// </summary>
        public string? DefaultFromAddress { get; set; }

        /// <summary>
        /// Default from name for the generic IEmailSender in DI
        /// </summary>
        public string? DefaultFromName { get; set; }
        
        /// <summary>
        /// Default tags (i.e. Office365 Categories) for the generic IEmailSender in DI
        /// Does nothing when using the Mailgun sender.
        /// </summary>
        public List<string>? DefaultTags { get; set; }

        /// <summary>
        /// Email sending provider - Mailgun, Office365 and Fake are only ones at this stage.
        /// </summary>
        public string Provider
        {
            get => _provider;
            set
            {
                PreCondition.RequiresNotNullOrWhitespace(value);
                // Ensure MailgunEmailSender is changed to Mailgun for backwards compatibility
                _provider = value.Replace("EmailSender", "", StringComparison.OrdinalIgnoreCase);   
            }
        }


        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Outcome Validate()
        {
            List<string> errors = new List<string>();
            if (!EmailSendingProviders.IsProviderSupported(Provider))
            {
                errors.Add(
                    $"{nameof(Provider)}: {Provider} is not supported. Supported providers are {string.Join(",", EmailSendingProviders.GetAllProviders())}");
            }
            return new Outcome(!errors.Any(), errors);
        }
        
    }
}
