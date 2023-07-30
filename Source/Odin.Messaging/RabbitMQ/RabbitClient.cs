using System;
using Odin.Logging;
using RabbitMQ.Client;

namespace Odin.Messaging.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitClient : IClient
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILoggerAdapter<RabbitClient> _logger;
        
        /// <summary>
        /// Connection configuration
        /// </summary>
        protected readonly RabbitSettings _configuration;
        
        /// <summary>
        /// RabbitMQ connection
        /// </summary>
        protected IConnection _connection;
        
        /// <summary>
        /// RabbitMQ channel
        /// </summary>
        protected IModel _channel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RabbitClient(ILoggerAdapter<RabbitClient> logger, RabbitSettings configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a new RabbitMQ Connection
        /// </summary>
        /// <returns></returns>
        protected IConnection CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _configuration.Host,
                VirtualHost = _configuration.VirtualHost,
                UserName = _configuration.UserName,
                Password = _configuration.UserPassword
            };

            string port = _configuration.Port;
            if (!string.IsNullOrWhiteSpace(port))
            {
                if (int.TryParse(port, out int portNumber))
                {
                    factory.Port = portNumber;
                }
            }

            IConnection connection = null;
            try
            {
                connection = factory.CreateConnection();
                if (connection == null)
                {
                    _errorStatus =
                        $"CreateConnection(): Unable to connect to RabbitMQ on Host={factory.HostName}, VHost={factory.VirtualHost}, RabbitUserName={factory.UserName}, Port={factory.Port}";
                    _logger.LogError(Status);
                }
            }
            catch (Exception err)
            {
                _errorStatus =
                    $"CreateConnection(): Unable to connect to RabbitMQ on Host={factory.HostName}, VHost={factory.VirtualHost}, RabbitUserName={factory.UserName}, Port={factory.Port}";
                _logger.LogError(Status, err);
            }

            return connection;
        }

        /// <summary>
        /// IsConnected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_connection == null) return false;
                if (!_connection.IsOpen) return false;
                if (_channel == null) return false;
                return _channel.IsOpen;
            }
        }


        /// <summary>
        /// Status
        /// </summary>
        public string Status
        {
            get
            {
                string mainStatus = "";
                if (!string.IsNullOrWhiteSpace(_errorStatus))
                {
                    mainStatus += "ERROR: " + _errorStatus + " ";
                }

                if (_channel != null)
                {
                    if (_channel.IsOpen)
                    {
                        mainStatus += "Channel is open. ";
                    }
                    else if (_channel.IsClosed)
                    {
                        mainStatus += "Channel is closed. ";
                    }
                }

                if (_connection != null)
                {
                    if (_connection.IsOpen)
                    {
                        mainStatus += $"Connection is open. ";
                    }
                    else
                    {
                        mainStatus += $"Connection is NOT open. ";
                    }
                }

                return mainStatus;
            }
        }

        /// <summary>
        /// EnsureConnected
        /// </summary>
        public void EnsureConnected()
        {
            try
            {
                bool reconnect = false;
                if (_connection == null)
                {
                    reconnect = true;
                }
                else if (!_connection.IsOpen)
                {
                    reconnect = true;
                }
                else if (_channel == null)
                {
                    reconnect = true;
                }
                else if (!_channel.IsOpen)
                {
                    reconnect = true;
                }

                if (reconnect)
                {
                    Connect();
                }
            }
            catch (Exception err)
            {
                _errorStatus += "EnsureConnected() unexpected exception. ";
                _logger.LogError("EnsureConnected() exception thrown", err);
            }
        }

        /// <summary>
        /// Error status text
        /// </summary>
        protected string _errorStatus;
        
        /// <summary>
        /// Connects to RabbitMQ, returning true if successfullP
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                _errorStatus = null;
                _connection = CreateConnection();
                if (_connection == null)
                {
                    return false;
                }
                _channel = _connection.CreateModel();
                if (_channel == null)
                {
                    _errorStatus = $"{nameof(Connect)}: Unable to open RabbitMQ channel. ";
                    _logger.LogError(Status);
                    return false;
                }
                return true;
            }
            catch (Exception err)
            {
                _errorStatus = $"{nameof(Connect)}: unexpected exception. ";
                _logger.LogError(Status, err);
                return false;
            }
        }

        /// <summary>
        /// EnsureDisconnected
        /// </summary>
        /// <returns></returns>
        public bool EnsureDisconnected()
        {
            try
            {
                if (_channel != null)
                {
                    _channel.Close();
                    _channel.Dispose();
                }

                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            catch (Exception err)
            {
                _errorStatus += "EnsureDisconnected() unexpected exception. ";
                _logger.LogError("Disconnect() exception thrown", err);
            }

            return true;
        }
    }
}