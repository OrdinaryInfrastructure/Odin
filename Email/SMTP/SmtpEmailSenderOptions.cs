using System.Net;
using System.Net.Mail;


namespace Odin.Email
{
    /// <summary>
    /// SMTP settings for loading from configuration 
    /// </summary>
    public sealed class SmtpEmailSenderOptions 
    {
        /// <summary>
        /// 'Smtp'
        /// </summary>
        public const string SmtpName = "Smtp";
        
        public string? Credentials { get; set;}

        public SmtpDeliveryFormat? DeliveryFormat { get; set;}

        public SmtpDeliveryMethod? DeliveryMethod { get; set;}

        public bool? EnableSsl { get; set;}

        public string? Host  { get; set;}

        public string? PickupDirectoryLocation { get; set;}

        public int? Port { get; set;}

        public ServicePoint? ServicePoint  { get; set;}

        public string? TargetName { get; set;}

        public int? Timeout { get; set;}

        public bool? UseDefaultCredentials { get; set;}
        
        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Result IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            return new Result(!errors.Any(), errors);
        }
        
    }
    
}
