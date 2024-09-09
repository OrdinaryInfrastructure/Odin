using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Odin.Messaging.RabbitMq;

/// <summary>
/// Helper client for a single RabbitMQ Connection (which corresponds to a single TCP connection). Each RabbitMQ Connection must correspond to a singleton RabbitConnectionService.
/// </summary>
public class RabbitConnectionService: IRabbitConnectionService
{
    
    private ConnectionFactory _connectionFactory;

    private long _maxChannels;

    private TimeSpan _sendTimeout;

    private string _clientProvidedName;

    public RabbitConnectionService(RabbitConnectionServiceSettings settings)
    {
        settings.Validate();
        _maxChannels = settings.MaxChannels;
        _sendTimeout = TimeSpan.FromMilliseconds(settings.SendTimeoutMillis);
        _clientProvidedName = settings.ConnectionName;
        _connectionFactory = new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true,
            // DispatchConsumersAsync = false,
            // ConsumerDispatchConcurrency = 0,
            HostName = settings.Host,
            Port = settings.Port,
            TopologyRecoveryEnabled = false,
            // ClientProperties = null,
            UserName = settings.Username,
            Password = settings.UserPassword,
            VirtualHost = settings.VirtualHost,
            ClientProvidedName = settings.ConnectionName,
        };
    }
    
    private IConnection? _connection;

    private readonly SemaphoreSlim _createConnectionSemaphore = new(1);
    
    private async Task<IConnection> GetConnection()
    {
        await _createConnectionSemaphore.WaitAsync();
        try
        {        
            ObjectDisposedException.ThrowIf(_isDisposing, typeof(RabbitConnectionService));
            
            if (_connection is not null)
            {
                return _connection;
            }
            
            _connection = _connectionFactory.CreateConnection();

            return _connection;
        }
        finally
        {
            _createConnectionSemaphore.Release();
        }
    }

    private readonly Dictionary<string, SingleExchangeSender> _senders = new();
    
    private readonly Dictionary<string, SingleQueueListener> _listeners = new();
    
    private readonly SemaphoreSlim _sendersSemaphore = new(1);

    private readonly SemaphoreSlim _listenersSemaphore = new(1);
    
    private long GetChannelsCount()
    {
        return _senders.Count + _listeners.Count;
    }


    private async Task<SingleExchangeSender> GetSingleExchangeSender(string exchangeName)
    {
        
        await _sendersSemaphore.WaitAsync();
        
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposing, typeof(RabbitConnectionService));
            
            if (_senders.TryGetValue(exchangeName, out var c))
            {
                return c;
            }
            
            var channelsCount = GetChannelsCount();
            
            if (channelsCount >= _maxChannels)
            {
                throw new Exception($"Will not create new SingleExchangeSender for exchange {exchangeName} as the MaxChannels limit, {_maxChannels}, has been reached.");
            }
            
            var connection = await GetConnection();

            var sender = new SingleExchangeSender(
                exchangeName: exchangeName,
                connection: connection,
                sendTimeout: _sendTimeout);
            
            _senders.Add(exchangeName, sender);
            
            return sender;
        }
        finally
        {
            _sendersSemaphore.Release();
        }

    }

    private async Task<SingleQueueListener> AddSingleQueueListener(string queueName, TimeSpan checkChannelPeriod, bool autoAck, ushort prefetchCount)
    {
        // Yes, we are locking this dictionary for the duration of the SingleQueueListener constructor, which involves several network round trips. Subscribing should not be done very often.
        await _listenersSemaphore.WaitAsync();

        try
        {
            ObjectDisposedException.ThrowIf(_isDisposing, typeof(RabbitConnectionService));
            
            if (_listeners.TryGetValue(queueName, out _))
            {
                throw new ApplicationException($"Listener for queue {queueName} already exists.");
            }

            if (GetChannelsCount() >= _maxChannels)
            {
                throw new ApplicationException($"Will not create new SingleQueueListener for queue {queueName} as the MaxChannels limit, {_maxChannels}, has been reached.");
            }

            var connection = await GetConnection();

            var listener = new SingleQueueListener(queueName, connection, checkChannelPeriod, autoAck, prefetchCount, _clientProvidedName);

            _listeners.Add(queueName, listener);

            return listener;
        }
        finally
        {
            _listenersSemaphore.Release();
        }
    }

    private async Task RemoveSingleQueueListener(string queueName)
    {

        await _listenersSemaphore.WaitAsync();
        try
        {
            if (_listeners.Remove(queueName, out var l))
            {
                l.Dispose();
            }
        }
        finally
        {
            _listenersSemaphore.Release();
        }
    }

    public async Task<IRabbitConnectionService.Subscription> SubscribeToConsume(string queueName, bool autoAck, ushort prefetchCount = 200,
        TimeSpan? channelCheckPeriod = null)
    {
        channelCheckPeriod ??= TimeSpan.FromSeconds(60);
        SingleQueueListener? listener = null;
        try
        {
            listener = await AddSingleQueueListener(queueName, channelCheckPeriod.Value, autoAck, prefetchCount);
        }
        catch (Exception)
        {
            listener?.Dispose();
            await RemoveSingleQueueListener(queueName);
            throw;
        }

        var subscription = new IRabbitConnectionService.Subscription
        {
            CloseChannel = async () =>
            {
                listener.Dispose();
                await RemoveSingleQueueListener(queueName);
            },
            StopConsuming = () =>
            {
                listener.CancelConsumer();
            }
        };

        listener.OnConsume += message =>
        {
            subscription.RaiseOnConsumed(message);
            return Task.CompletedTask;
        };

        listener.OnFailure += ex =>
        {
            subscription.RaiseOnFailure(ex);
            return Task.CompletedTask;
        };
        
        return subscription;
    }


    public async Task SendAsync(string exchangeName, string routingKey, Dictionary<string, object> headers, string contentType, byte[] body, bool persistentDelivery = true, bool mandatoryRouting = false)
    {
        var sender = await GetSingleExchangeSender(exchangeName);
        var task = sender.EnqueueMessage(routingKey, headers, contentType, body, persistentDelivery, mandatoryRouting);
        await task;
    }

    private bool _isDisposing = false;
    
    public async ValueTask DisposeAsync()
    {
        await _sendersSemaphore.WaitAsync();
        await _listenersSemaphore.WaitAsync();
        await _createConnectionSemaphore.WaitAsync();
        
        _isDisposing = true;
        
        foreach (var sender in _senders.Values)
        {
            try
            {
                sender.Dispose();
            }
            catch
            {
            }
        }

        foreach (var listener in _listeners.Values)
        {
            try
            {
                listener.Dispose();
            }
            catch
            {
            }
        }

        try
        {
            _connection?.Close();
        }
        catch
        {
        }
        
        _sendersSemaphore.Release();
        _listenersSemaphore.Release();
        _createConnectionSemaphore.Release();
    }


}