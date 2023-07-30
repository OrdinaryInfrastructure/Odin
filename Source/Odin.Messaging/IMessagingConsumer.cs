using System;

namespace Odin.Messaging
{
    /// <summary>
    /// Represents a consuming client of a messaging service
    /// </summary>
    public interface IMessagingConsumer : IClient
    {
        /// <summary>
        /// Connect to a queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="receivingMessageHandlers"></param>
        /// <returns></returns>
        bool Connect(string queueName, EventHandler<string> receivingMessageHandlers);

        /// <summary>
        /// The name of the queue
        /// </summary>
        string QueueName { get; }
        
    }
}
