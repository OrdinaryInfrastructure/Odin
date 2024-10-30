using Odin.Email;
using Odin.System;


namespace Odin.Notifications
{
    /// <summary>
    /// EmailNotifierOptions for loading from configuration 
    /// </summary>
    public sealed class EmailNotifierOptions 
    {
        /// <summary>
        /// SubjectPrefix
        /// </summary>
        public string SubjectPrefix { get; set; }
        
        /// <summary>
        /// Recipient emails
        /// </summary>
        public string ToEmails { get; set; }
        
        /// <summary>
        /// From email address to send from.
        /// </summary>
        public string FromEmail { get; set; }
        
        /// <summary>
        /// Returns ToEmails as a list, or null if none.
        /// </summary>
        /// <returns></returns>
        public List<EmailAddress> GetToEmails()
        {
            if (string.IsNullOrWhiteSpace(ToEmails)) return null;
            return ToEmails.Split(',', ';').Select(c => new EmailAddress(c)).ToList();
        }

        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Outcome IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(ToEmails)) errors.Add($"{nameof(ToEmails)} is not specified"); 
            if (string.IsNullOrWhiteSpace(FromEmail)) errors.Add($"{nameof(FromEmail)} is not specified"); 
            return new Outcome(!errors.Any(), errors);
        }


    }
}
