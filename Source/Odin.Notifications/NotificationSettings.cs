﻿using Odin.System;


namespace Odin.Notifications
{
    /// <summary>
    /// Notifications settings for loading from configuration 
    /// </summary>
    public sealed class NotificationSettings 
    {
        /// <summary>
        /// Notification sender provider enumeration
        /// </summary>
        public string Provider { get; set; } 
        
        /// <summary>
        /// Validates the settings instance
        /// </summary>
        /// <returns></returns>
        public Outcome IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Provider))
            {
                errors.Add("Notifications:Provider is not specified");
            } 
            else if (!Providers.AllProviders().Contains(Provider))
            {
                errors.Add($"Notifications:Provider is not recognised ({Provider})");
            }
            return new Outcome(!errors.Any(), errors);
        }
        
    }
}
