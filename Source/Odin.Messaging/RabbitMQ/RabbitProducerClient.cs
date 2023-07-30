using System;
using System.Collections.Generic;
using System.Text;
using Odin.DesignContracts;
using Odin.Logging;
using RabbitMQ.Client;

namespace Odin.Messaging.RabbitMQ
{
    /// <summary>
    /// For sending message to RabbitMQ
    /// </summary>
    public sealed class RabbitMessagingProducer : RabbitClient, IMessagingProducer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RabbitMessagingProducer(ILoggerAdapter<RabbitClient> logger, RabbitSettings configuration) : base(logger,
            configuration)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="headers"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public void Publish(string exchangeName, string routingKey, Dictionary<string, object> headers, string message)
        {
            PreCondition.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(exchangeName), nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(message)) return;
            EnsureConnected();
            byte[] body = Encoding.UTF8.GetBytes(message);
            IBasicProperties propsWithHeaders = null;
            if (headers != null && headers.Count>0)
            {
                propsWithHeaders = CreateBasicProperties();
                propsWithHeaders.Headers = new Dictionary<string, object>();
                foreach (KeyValuePair<string,object> keyValuePair in headers)
                {
                    propsWithHeaders.Headers.Add(keyValuePair.Key,  keyValuePair.Value);
                }
            }
            _channel.BasicPublish(exchangeName,routingKey, propsWithHeaders, body);
        }

        /// <summary>
        /// Creates basic properties from the current channel...
        /// </summary>
        /// <returns></returns>
        internal IBasicProperties CreateBasicProperties()
        {
            EnsureConnected();
            return _channel.CreateBasicProperties();
        }
    }
}