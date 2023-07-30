using System.Collections.Generic;

namespace Odin.Messaging
{
    /// <summary>
    /// Represents a producing client of a messaging service
    /// </summary>
    public interface IMessagingProducer : IClient
    {
        /// <summary>
        /// Connect to a queue
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="headers"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        void Publish(string exchangeName, string routingKey, Dictionary<string, object> headers, string message);

    }
}
