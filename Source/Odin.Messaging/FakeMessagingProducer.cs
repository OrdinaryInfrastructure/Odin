using System.Collections.Generic;

namespace Odin.Messaging
{
    /// <summary>
    /// Does nothing 
    /// </summary>
    public sealed class FakeMessagingProducer : IMessagingProducer
    {
        /// <summary>
        /// EnsureDisconnected
        /// </summary>
        /// <returns></returns>
        public bool EnsureDisconnected()
        {
            return true;
        }

        /// <summary>
        /// Returns true if the consumer is connected
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status
        {
            get { return "OK"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnsureConnected()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="headers"></param>
        /// <param name="message"></param>
        public void Publish(string exchangeName, string routingKey, Dictionary<string, object> headers, string message)
        {
        }

    }
}