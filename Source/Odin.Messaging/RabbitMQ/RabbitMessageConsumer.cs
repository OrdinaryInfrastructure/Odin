// using System;
// using System.Text;
// using Odin.DesignContracts;
// using Odin.Logging;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
//
// namespace Odin.Messaging
// {
//     /// <summary>
//     /// RabbitMQ implementation of IMessageConsumer
//     /// </summary>
//     public sealed class RabbitMessageConsumer : IMessageConsumer
//     {
//         private readonly ILoggerAdapter<IMessageConsumer> _logger;
//         private readonly RabbitSettings _settings;
//         private IConnection _connection;
//         private IModel _channel;
//         private EventHandler<BasicDeliverEventArgs> _handler;
//         private string _consumerTag;
//
//         /// <summary>
//         /// Default constructor
//         /// </summary>
//         /// <param name="settings"></param>
//         /// <param name="logger"></param>
//         public RabbitMessageConsumer(RabbitSettings settings, ILoggerAdapter<IMessageConsumer> logger)
//         {
//             _logger = logger;
//             _settings = settings;
//         }
//
//         private IConnection CreateConnection()
//         {
//             ConnectionFactory factory = new ConnectionFactory()
//             {
//                 HostName = _settings.Host,
//                 VirtualHost = _settings.VirtualHost,
//                 UserName = _settings.UserName,
//                 Password = _settings.Password
//             };
//
//             if (!string.IsNullOrWhiteSpace(_settings.Port))
//             {
//                 if (int.TryParse(_settings.Port, out int portNumber))
//                 {
//                     factory.Port = portNumber;
//                 }
//             }
//
//             IConnection connection = null;
//             try
//             {
//                 connection = factory.CreateConnection();
//                 if (connection == null)
//                 {
//                     _logger.LogError($"{nameof(CreateConnection)}: Unable to connect to RabbitMQ.\nRabbitHost:{factory.HostName}\nRabbitVirtualHost:{factory.VirtualHost}\nRabbitUserName:{factory.UserName}\nRabbitUserPassword:{factory.Password}\nRabbitPort:{factory.Port}");
//                 }
//             }
//             catch (Exception err)
//             {
//                 _logger.LogError($"{nameof(CreateConnection)}: Unable to connect to RabbitMQ.\nRabbitHost:{factory.HostName}\nRabbitVirtualHost:{factory.VirtualHost}\nRabbitUserName:{factory.UserName}\nRabbitUserPassword:{factory.Password}\nRabbitPort:{factory.Port}", err);
//             }
//             return connection;
//         }
//
//         /// <summary>
//         /// Returns true if the consumer is connected
//         /// </summary>
//         public bool IsConnected
//         {
//             get
//             {
//                 if (_connection == null) return false;
//                 if (!_connection.IsOpen) return false;
//                 if (_channel == null) return false;
//                 return _channel.IsOpen;
//             }
//         }
//
//         /// <summary>
//         /// Connects the consumer
//         /// </summary>
//         /// <param name="queueName"></param>
//         /// <param name="receivingMessageHandlers"></param>
//         /// <returns></returns>
//         public bool Connect(string queueName, EventHandler<BasicDeliverEventArgs> receivingMessageHandlers)
//         {
//             PreCondition.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(queueName), nameof(queueName));
//             PreCondition.Requires<ArgumentNullException>(receivingMessageHandlers != null, nameof(receivingMessageHandlers));
//
//             // Todo: Handle scenario of already being connected.
//             // Todo: Handle errors...
//
//             try
//             {
//                 _handler = receivingMessageHandlers;
//                 _connection = CreateConnection();
//                 if (_connection == null)
//                 {
//                     return false;
//                 }
//                 _channel = _connection.CreateModel();
//                 if (_channel == null)
//                 {
//                     _logger.LogError("CreateConnection(): Unable to create Rabbit channel");
//                     return false;
//                 }
//                 EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
//                 consumer.Received += ConsumerReceived;
//                 _consumerTag = _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
//                 return true;
//             }
//             catch (Exception err)
//             {
//                 _logger.LogError("CreateConnection(): Exception occurred connecting to RabbitMQ.", err);
//                 Disconnect();
//                 return false;
//             }
//         }
//
//         /// <summary>
//         /// Disconnects the consumer
//         /// </summary>
//         /// <returns></returns>
//         public bool Disconnect()
//         {
//             if (_channel != null)
//             {
//                 _channel.Close();
//                 _channel.Dispose();
//             }
//
//             if (_connection != null)
//             {
//                 _connection.Close();
//                 _connection.Dispose();
//             }
//
//             _consumerTag = null;
//             _handler = null;
//             return true;
//         }
//
//         private void ConsumerReceived(object sender, BasicDeliverEventArgs ea)
//         {
//             try
//             {
//                 // string message = Encoding.UTF8.GetString(ea.Body);
//                 try
//                 {
//                     _handler(this, ea);
//                 }
//                 catch (Exception err2)
//                 {
//                     _logger.LogError($"RabbitMessageConsumer:ConsumerReceived handler exception: \n{Encoding.UTF8.GetString(ea.Body.Span)}", err2);
//                 }
//
//                 // _logger.LogTrace("Received {0}", message);
//             }
//             catch (Exception err)
//             {
//                 _logger.LogError("RabbitMessageConsumer:ConsumerReceived decoding exception", err);
//             }
//         }
//     }
// }
//
