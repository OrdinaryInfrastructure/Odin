using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Odin.Messaging.RabbitMq;

internal class SingleQueueListener: IDisposable
{

    private readonly string _queueName;

    private readonly TimeSpan _checkChannelPeriod;
    
    private readonly IModel _channel;

    private readonly EventingBasicConsumer _consumer;

    private readonly bool _autoAck;
    
    public event Func<Exception, Task>? OnFailure;
    public event Func<IRabbitConnectionService.ConsumedMessage, Task>? OnConsume;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    public SingleQueueListener(string queueName, IConnection connection, TimeSpan checkChannelPeriod, bool autoAck, ushort prefetchCount, string clientName)
    {
        _queueName = queueName;
        _checkChannelPeriod = checkChannelPeriod;
        _autoAck = autoAck;

        _channel = connection.CreateModel();
        // PrefetchSize 0 means no limit on the number of octets of data the broker will send.
        _channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: true );

        _channel.QueueDeclarePassive(queueName);

        if (!_channel.IsOpen)
        {
            throw new Exception($"Failed to open channel. Reason: " + _channel.CloseReason?.ReplyText);
        }

        _consumer = new EventingBasicConsumer(_channel);

        var consumerTag = $"RabbitConnectionService-SingleQueueListener-{clientName}-{_queueName}-{Guid.NewGuid().ToString().Substring(0,4)}";

        _channel.BasicConsume(
            consumer: _consumer,
            queue: queueName,
            autoAck: _autoAck,
            consumerTag: consumerTag);

        _consumer.ConsumerCancelled += GetConsumerCancelledHandler();
        _consumer.Received += GetDeliverEventHandler();
        _channel.ModelShutdown += GetChannelShutdownHandler();

        _ = CheckChannelPeriodically(_cancellationTokenSource.Token);
    }


    private EventHandler<BasicDeliverEventArgs> GetDeliverEventHandler()
    {
        return (sender, args) =>
        {
            var message = new IRabbitConnectionService.ConsumedMessage
            {
                Body = args.Body.ToArray(),
                ConsumedAt = DateTimeOffset.Now,
                IsRedelivered = args.Redelivered,
                Exchange = args.Exchange,
                RoutingKey = args.RoutingKey,
                ConsumerTag = args.ConsumerTag,
                CorrelationId = args.BasicProperties?.CorrelationId,
                Headers = args.BasicProperties?.Headers?.ToDictionary() ?? new Dictionary<string, object>(),
                MessageId = args.BasicProperties?.MessageId,
                ReplyTo = args.BasicProperties?.ReplyTo,
                Type = args.BasicProperties?.Type,
                Timestamp = args.BasicProperties?.Timestamp.UnixTime is null ? null : DateTimeOffset.FromUnixTimeSeconds(args.BasicProperties.Timestamp.UnixTime),
                Ack = _autoAck ? null : GetManualAckHandler(args.DeliveryTag),
                Nack = _autoAck ? null : GetManualNackHandler(args.DeliveryTag),
            };
            
            OnConsume?.Invoke(message);
        };
    }

    private Action GetManualAckHandler(ulong deliveryTag)
    {
        return () =>
        {
            _channel.BasicAck(deliveryTag, false);
        };
    }

    private Action<bool> GetManualNackHandler(ulong deliveryTag)
    {
        return shouldRequeue =>
        {
            _channel.BasicNack(deliveryTag, false, shouldRequeue);
        };
    }

    private async Task CheckChannelPeriodically(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(_checkChannelPeriod, token);
            
            if (token.IsCancellationRequested)
            {
                break;
            }
            
            Console.WriteLine("Periodically checking channel...");
            
            if (!_channel.IsOpen)
            {
                OnFailure?.Invoke(new Exception($"A periodic check found the channel closed. Reason: " + _channel.CloseReason.ReplyText));
                Dispose();
            }

            if (!_consumer.IsRunning)
            {
                OnFailure?.Invoke(new Exception($"A periodic check found the consumer not running. ShutdownReason: " + _consumer.ShutdownReason.ReplyText));
                Dispose();
            }
        }
    }

    private EventHandler<ConsumerEventArgs> GetConsumerCancelledHandler()
    {
        return (sender, args) =>
        {
            var exception = new Exception($"Consumer was cancelled. Cancelled tags: {string.Join(", ", args.ConsumerTags)}. ShutdownReason: " + _consumer.ShutdownReason);
            OnFailure?.Invoke(exception);
            Dispose();
        };
    }

    private EventHandler<ShutdownEventArgs> GetChannelShutdownHandler()
    {
        return (sender, args) =>
        {
            var exception = new Exception($"Channel shut down. Shutdown reason: {args.ReplyText} CloseReason: " + _channel.CloseReason?.ReplyText);
            OnFailure?.Invoke(exception);
            Dispose();
        };
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        foreach (var tag in _consumer.ConsumerTags)
        {
            try
            {
                _channel.BasicCancel(tag);
            }
            catch
            {
            }
        }

        try
        {
            _channel.Close();
        }
        catch
        {
        }
    }
    
}