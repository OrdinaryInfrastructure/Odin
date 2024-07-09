using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Odin.Messaging.RabbitMq;

/// <summary>
/// Helper client for a single RabbitMQ Connection (which corresponds to a single TCP connection). Each RabbitMQ Connection must correspond to a singleton RabbitConnectionService.
/// </summary>
public class RabbitConnectionService: IRabbitConnectionService, IDisposable
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
    
    private async Task<IConnection> GetConnection()
    {
        if (_connection is not null)
        {
            return _connection;
        }
        
        await Task.Delay(1);
        
        _connection = _connectionFactory.CreateConnection();
        
        return _connection;
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
            if (_listeners.TryGetValue(queueName, out _))
            {
                throw new Exception($"Listener for queue {queueName} is already active.");
            }

            if (GetChannelsCount() >= _maxChannels)
            {
                throw new Exception($"Will not create new SingleQueueListener for queue {queueName} as the MaxChannels limit, {_maxChannels}, has been reached.");
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
            _listeners.Remove(queueName);
        }
        finally
        {
            _listenersSemaphore.Release();
        }
    }

    public async Task<Func<Task>> SubscribeToConsume(string queueName, Func<Exception, Task> onFailure, Func<IRabbitConnectionService.ConsumedMessage, Task> onConsume, bool autoAck, ushort prefetchCount = 200,
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

        listener.OnConsume += message =>
        {
            _ = onConsume(message);
            return Task.CompletedTask;
        };

        listener.OnFailure += ex =>
        {
            _ = onFailure(ex);
            return Task.CompletedTask;
        };

        listener.OnDisposed += () =>
        {
            _ = RemoveSingleQueueListener(queueName);
            return Task.CompletedTask;
        };

        return async () =>
        {
            listener.Dispose();
            await RemoveSingleQueueListener(queueName);
        };

    }


    public async Task SendAsync(string exchangeName, string routingKey, Dictionary<string, object> headers, string contentType, byte[] body, bool persistentDelivery = true, bool mandatoryRouting = false)
    {
        var sender = await GetSingleExchangeSender(exchangeName);
        var task = sender.EnqueueMessage(routingKey, headers, contentType, body, persistentDelivery, mandatoryRouting);
        await task;
    }
    
    
    
    public void Dispose()
    {
        foreach (var sender in _senders.Values)
        {
            sender.Dispose();
        }
        _connection?.Close();
    }


}