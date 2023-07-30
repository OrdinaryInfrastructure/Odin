// using System;
// using System.Text;
// using Odin.DesignContracts;
// using Odin.Logging;
// using Microsoft.Extensions.Configuration;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
//
// namespace Odin.Messaging
// {
//     public sealed class RabbitConsumerClient : RabbitClient, IConsumerClient
//     {
//         public string QueueName { get; private set; }
//
//         private EventHandler<string> _handler;
//         private string _consumerTag;
//
//         public RabbitConsumerClient(ILoggerAdapter<RabbitClient> logger, RabbitSettings configuration) : base(logger, configuration)
//         {
//         }
//
//         public bool Connect(string queueName, EventHandler<string> receivingMessageHandlers)
//         {
//             PreCondition.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(queueName), nameof(queueName));
//             PreCondition.Requires<ArgumentNullException>(receivingMessageHandlers != null,
//                 nameof(receivingMessageHandlers));
//
//             // Todo: Handle scenario of already being connected.
//             // Todo: Handle errors...
//
//             try
//             {
//                 _errorStatus = null;
//                 _handler = receivingMessageHandlers;
//                 _connection = CreateConnection();
//                 QueueName = queueName;
//                 if (_connection == null)
//                 {
//                     return false;
//                 }
//
//                 _channel = _connection.CreateModel();
//                 if (_channel == null)
//                 {
//                     _errorStatus = "Connect(): Unable to create Rabbit channel. ";
//                     _logger.LogError(Status);
//                     return false;
//                 }
//
//                 EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
//                 consumer.Received += ConsumerReceived;
//                 _consumerTag = _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
//                 return true;
//             }
//             catch (Exception err)
//             {
//                 _errorStatus = "Connect() unexpected exception. ";
//                 _logger.LogError(Status, err);
//                 return false;
//             }
//         }
//
//         private void ConsumerReceived(object sender, BasicDeliverEventArgs ea)
//         {
//             try
//             {
//                 string message = Encoding.UTF8.GetString(ea.Body.Span);
//
//                 try
//                 {
//                     _handler(this, message);
//                 }
//                 catch (Exception err2)
//                 {
//                     _logger.LogError($"RabbitClient:ConsumerReceived handler exception: \n{message}", err2);
//                 }
//             }
//             catch (Exception err)
//             {
//                 _logger.LogError("RabbitClient:ConsumerReceived decoding exception", err);
//             }
//         }
//     }
// }