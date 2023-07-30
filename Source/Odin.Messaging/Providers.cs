using System.Collections.Generic;

namespace Odin.Messaging
{
    /// <summary>
    /// The messaging providers supported
    /// </summary>
    public static class Providers
    {
        /// <summary>
        /// Fake provider for testing...
        /// </summary>
        public const string FakeMessaging = "FakeMessaging";
        
        /// <summary>
        /// RabbitMQ
        /// </summary>
        public const string RabbitMQ = "RabbitMQ";

        /// <summary>
        /// Returns a list of supported providers...
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSupportedProviders()
        {
             return new List<string> {RabbitMQ,FakeMessaging};
        }
    }
}