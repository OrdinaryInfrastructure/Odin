using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Odin.Messaging.RabbitMq;

internal class SingleQueueListener: IAsyncDisposable
{
    private readonly string _queueName;

    private readonly string _consumerTag;

    private readonly TimeSpan _checkChannelPeriod;

    private readonly TimeSpan _channelOperationsTimeout;
    
    private readonly IModel _channel;

    private readonly EventingBasicConsumer _consumer;

    private readonly bool _autoAck;
    private readonly bool _exclusive;
    
    public event Func<Exception, Task>? OnFailure;
    public event Func<IRabbitConnectionService.ConsumedMessage, Task>? OnConsume;

    private CancellationTokenSource _checkChannelCts;
    private CancellationTokenSource _checkConsumerCts;
    
    public SingleQueueListener(string queueName, IConnection connection, TimeSpan checkChannelPeriod, bool autoAck, bool exclusive, ushort prefetchCount, string clientName, TimeSpan? channelOperationsTimeout = null)
    {
        _queueName = queueName;
        _checkChannelPeriod = checkChannelPeriod;
        _autoAck = autoAck;
        _exclusive = exclusive;
        _channelOperationsTimeout = channelOperationsTimeout ?? TimeSpan.FromSeconds(5);
        _checkChannelCts = new CancellationTokenSource();
        _checkConsumerCts = new CancellationTokenSource();
        
        _channel = connection.CreateModel();
        // PrefetchSize 0 means no limit on the number of octets of data the broker will send.
        _channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: true );

        _channel.QueueDeclarePassive(queueName);

        if (!_channel.IsOpen)
        {
            throw new ApplicationException("Failed to open channel. Reason: " + _channel.CloseReason?.ReplyText);
        }

        _consumer = new EventingBasicConsumer(_channel);
        
        _consumer.ConsumerCancelled += HandleConsumerCancelled;
        _consumer.Received += HandleMessageReceived;
        _channel.ModelShutdown += HandleChannelShutdown;

        _consumerTag = $"{clientName}-{_queueName}-{Guid.NewGuid().ToString().Substring(0,4)}";

        _ = CheckChannelPeriodically();
    }

    public async Task StartConsuming()
    {
        if (!_channel.IsOpen)
        {
            throw new ApplicationException("Cannot start consuming since channel is not open.");
        }

        if (_consumer.IsRunning)
        {
            return;
        }

        TaskCompletionSource tcs = new TaskCompletionSource();

        Task timeoutTask = Task.Delay(_channelOperationsTimeout);

        _consumer.Registered += (obj, args) =>
        {
            if (args.ConsumerTags.Length == 1 && args.ConsumerTags[0] == _consumerTag)
            {
                tcs.TrySetResult();
                return;
            }

            tcs.TrySetException(new ApplicationException($"Consume-ok fired with unexpected ConsumerTags (expected [{_consumerTag}]): [{string.Join(", ", args.ConsumerTags)}]"));
        };
        
        _channel.BasicConsume(
            consumer: _consumer, 
            queue: _queueName, 
            autoAck: _autoAck, 
            consumerTag: _consumerTag,
            exclusive: _exclusive);

        Task completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            throw new TimeoutException($"StartConsuming reached timeout of {_channelOperationsTimeout}");
        }

        _ = CheckConsumerPeriodically();
    }
    

    void HandleConsumerRegistered(object? sender, ConsumerEventArgs args)
    {
        Console.WriteLine("HandleConsumerRegistered fired. Tags: " + string.Join(", ", args.ConsumerTags));
    }
    
    void HandleConsumerUnRegistered(object? sender, ConsumerEventArgs args)
    {
        Console.WriteLine("HandleConsumerUnregistered fired. Tags: " + string.Join(", ", args.ConsumerTags));
    }
    
    private void HandleMessageReceived(object? sender, BasicDeliverEventArgs args)
    {
        IRabbitConnectionService.ConsumedMessage message = new IRabbitConnectionService.ConsumedMessage
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
            AckNackCallbacks = _autoAck ? null : new IRabbitConnectionService.ConsumedMessage.AcknowledgementCallbacks
            {
                Ack = GetManualAckHandler(args.DeliveryTag),
                Nack = GetManualNackHandler(args.DeliveryTag),
            },
        };
            
        OnConsume?.Invoke(message);
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

    private async Task CheckChannelPeriodically()
    {
        while (!_checkChannelCts.Token.IsCancellationRequested)
        {
            await Task.Delay(_checkChannelPeriod, _checkChannelCts.Token);
            if (!_channel.IsOpen)
            {
                OnFailure?.Invoke(new ApplicationException($"A periodic check found the channel closed. Reason: " + _channel.CloseReason.ReplyText));
            }
        }
    }

    private async Task CheckConsumerPeriodically()
    {
        while (!_checkConsumerCts.Token.IsCancellationRequested)
        {
            await Task.Delay(_checkChannelPeriod, _checkConsumerCts.Token);
            if (!_consumer.IsRunning)
            {
                OnFailure?.Invoke(new IRabbitConnectionService.ConsumerCancelledException($"A periodic check found the consumer not running. ShutdownReason: " + _consumer.ShutdownReason.ReplyText));
            }
        }
    }
    
    private void HandleConsumerCancelled(object? sender, ConsumerEventArgs args)
    {
        IRabbitConnectionService.ConsumerCancelledException exception = new IRabbitConnectionService.ConsumerCancelledException($"Consumer was cancelled. Cancelled tags: {string.Join(", ", args.ConsumerTags)}. ShutdownReason: " + _consumer.ShutdownReason);
        OnFailure?.Invoke(exception);
    }
    
    private void HandleChannelShutdown(object? sender, ShutdownEventArgs args)
    {
        ApplicationException exception = new ApplicationException($"Channel shut down. Shutdown reason: {args.ReplyText} CloseReason: " + _channel.CloseReason?.ReplyText);
        OnFailure?.Invoke(exception);
    }
    
    public async Task StopConsuming()
    {
        await _checkConsumerCts.CancelAsync();

        _consumer.ConsumerCancelled -= HandleConsumerCancelled;

        if (!_channel.IsOpen)
        {
            throw new ApplicationException("Cannot cancel consuming since channel is not open.");
        }

        TaskCompletionSource tcs = new TaskCompletionSource();

        Task timeoutTask = Task.Delay(_channelOperationsTimeout);

        _consumer.Unregistered += (obj, args) =>
        {
            if (args.ConsumerTags.Length == 1 && args.ConsumerTags[0] == _consumerTag)
            {
                tcs.TrySetResult();
                return;
            }

            tcs.TrySetException(new ApplicationException($"Consume-cancel-ok fired with unexpected ConsumerTags (expected [{_consumerTag}]): [{string.Join(", ", args.ConsumerTags)}]"));
        };

        _channel.BasicCancel(_consumerTag);

        Task completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            throw new TimeoutException($"StopConsuming reached timeout of {_channelOperationsTimeout}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await StopConsuming();
        }
        catch
        {
        }

        await _checkChannelCts.CancelAsync();
        _channel.ModelShutdown -= HandleChannelShutdown;
        try
        {
            _channel.Close();
        }
        catch
        {
        }
        _channel.Dispose();
    }
}