using System.Collections.Generic;
using System.Linq;
using Odin.System;


namespace Odin.Messaging.RabbitMQ
{
    /// <summary>
    /// Represents configuration for a RabbitMQ client
    /// </summary>
    public sealed class RabbitSettings 
    {
        /// <summary>
        /// IP or host name of RabbitMQ host
        /// </summary>
        public string Host { get; set; }
        
        /// <summary>
        /// VHost. Default is '/'
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        
        /// <summary>
        /// Connection UserName
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Connection password
        /// </summary>
        public string UserPassword { get; set; }
        
        /// <summary>
        /// Connection port. Default is 5672
        /// </summary>
        public string Port { get; set; } = "5672";

        /// <summary>
        /// Validates the settings for errors
        /// </summary>
        /// <returns></returns>
        public Outcome IsConfigurationValid()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Host)) errors.Add("Host is not specified"); 
            if (string.IsNullOrWhiteSpace(VirtualHost)) errors.Add("VirtualHost is not specified"); 
            if (string.IsNullOrWhiteSpace(UserName)) errors.Add("UserName is not specified"); 
            if (string.IsNullOrWhiteSpace(UserPassword)) errors.Add("UserPassword is not specified"); 
            if (string.IsNullOrWhiteSpace(Port)) errors.Add("Port is not specified"); 
            return new Outcome(!errors.Any(), errors);
        }

    }
}
