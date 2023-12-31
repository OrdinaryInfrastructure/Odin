﻿using System;
using System.Linq;

namespace Odin.Email
{
    /// <summary>
    /// The IEmailSender providers supported
    /// </summary>
    public static class EmailSendingProviders
    {
        /// <summary>
        /// Fake provider for testing...
        /// </summary>
        public const string Fake = "Fake";
        
        /// <summary>
        /// Mailgun
        /// </summary>
        public const string Mailgun = "Mailgun";

        /// <summary>
        /// Returns the supported provider names
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllProviders()
        {
            return new[] {Mailgun, Fake};
        }
        
        /// <summary>
        /// Indicates with the name is a supported 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static bool IsProviderSupported(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName)) return false;
            if (providerName.EndsWith("EmailSender", StringComparison.OrdinalIgnoreCase))
            {
                providerName = providerName.Replace("EmailSender", "");
            }
            return GetAllProviders().Any(c =>c.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        }

    }
}