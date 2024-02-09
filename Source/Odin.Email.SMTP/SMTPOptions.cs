using System.Collections.Generic;
using System.Linq;
using Odin.System;


namespace Odin.Email
{
    /// <summary>
    /// SMTP settings for loading from configuration 
    /// </summary>
    public sealed class SMTPOptions 
    {
        /// <summary>
        /// 'SMTP'
        /// Also the default EmailSending configuration section name for SMTP
        /// </summary>
        public const string SMTPName = "SMTP";

        
        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Outcome IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            return new Outcome(!errors.Any(), errors);
        }
        
    }
    
}
