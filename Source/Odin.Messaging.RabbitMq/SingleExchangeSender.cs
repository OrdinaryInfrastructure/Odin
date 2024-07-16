using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Odin.Messaging.RabbitMq;

/// <summary>
/// Encapsulates a single RabbitMQ channel for publishing only, which corresponds to a distinct Exchange name.
/// Automatically re-opens the Channel if it closed.
/// </summary>
internal class SingleExchangeSender: IDisposable
{
    public const byte TransientDeliveryMode = 1;
    public const byte PersistentDeliveryMode = 2;
    private static TimeSpan RedeclareExchangeInterval { get; } = TimeSpan.FromSeconds(60);

    private CancellationTokenSource _cancellationTokenSource = new();

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        foreach (var message in MessagesPendingPublish.Concat(PublishedMessagesByMessageId.Values).Concat(PublishedMessagesBySeqNo.Values))
        {
            message.TaskCompletionSource.TrySetCanceled();
        }
        Channel?.Close();
    }
    private record PendingMessage
    {
        public ulong? PublishSeqNo { get; set; }
        
        public required string MessageId { get; init; }
        
        public required TaskCompletionSource TaskCompletionSource { get; init; }
        
        public required string ExchangeName { get; init; }
        
        public required string RoutingKey { get; init; }

        public Dictionary<string, object> Headers { get; init; } = new();
        
        public required ReadOnlyMemory<byte> Body { get; init; }
        
        public required bool Mandatory { get; init; }
        
        public required bool PersistentDelivery { get; init; }
        
        public required string ContentType { get; init; }

        public enum RoutingConfirmationStatus
        {
            NotStarted,
            AwaitingReturn,
            MessageReturned,
        }

        public RoutingConfirmationStatus RoutingConfirmation { get; set; } = RoutingConfirmationStatus.NotStarted;

        public enum PublishConfirmationStatus
        {
            NotStarted,
            AwaitingAckOrNack,
            Acked,
            Nacked,
        }

        public PublishConfirmationStatus PublishConfirmation { get; set; } = PublishConfirmationStatus.NotStarted;
        
        public bool HasTimedOut { get; set; } = false;
    }

    private TimeSpan _sendTimeout;
    
    public string ExchangeName;

    private IConnection _connection;

    private IModel? Channel;

    private DateTimeOffset? ExchangeRedeclaredAt;

    private ConcurrentQueue<PendingMessage> MessagesPendingPublish { get; } = new();

    /// <summary>
    /// For publish confirms
    /// </summary>
    private ConcurrentDictionary<ulong, PendingMessage> PublishedMessagesBySeqNo { get; } = new();
        
    /// <summary>
    /// For mandatory confirms
    /// </summary>
    private ConcurrentDictionary<string, PendingMessage> PublishedMessagesByMessageId { get; } = new();
    
    private SemaphoreSlim MessageEnqueuedSemaphore { get; } = new SemaphoreSlim(0);

    public SingleExchangeSender(string exchangeName, IConnection connection, TimeSpan sendTimeout)
    {
        
        ExchangeName = exchangeName;
        _connection = connection;
        _sendTimeout = sendTimeout;
        Task.Run(() => ProcessMessagesPendingPublish(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        // Task.Run(() => ReportDeltas(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
    }

    public Task EnqueueMessage(string routingKey, Dictionary<string, object> headers, string contentType, byte[] body, bool persistentDelivery, bool mandatory)
    {
        var message = new PendingMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            TaskCompletionSource = new TaskCompletionSource(),
            ExchangeName = ExchangeName,
            RoutingKey = routingKey,
            Headers = headers,
            ContentType = contentType,
            Body = body,
            PersistentDelivery = persistentDelivery,
            Mandatory = mandatory,
        };
        MessagesPendingPublish.Enqueue(message);
        MessageEnqueuedSemaphore.Release();
        return message.TaskCompletionSource.Task;
    }

    private async Task TimeoutMessage(PendingMessage message, CancellationToken token)
    {
        await Task.Delay(_sendTimeout, token);

        message.HasTimedOut = true;
        
        TryFinaliseMessage(message);
    }
    
    private void HandleMessagePublished(ulong seqNo, string messageId, PendingMessage message)
    {
        message.PublishConfirmation = PendingMessage.PublishConfirmationStatus.AwaitingAckOrNack;
        message.RoutingConfirmation = PendingMessage.RoutingConfirmationStatus.AwaitingReturn;
        PublishedMessagesBySeqNo.TryAdd(seqNo, message);
        PublishedMessagesByMessageId.TryAdd(messageId, message);
        _ = TimeoutMessage(message, _cancellationTokenSource.Token);
    }
    
    private void TryRemoveMessage(PendingMessage message)
    {
        PublishedMessagesByMessageId.TryRemove(message.MessageId, out _);
        if (message.PublishSeqNo.HasValue)
        {
            PublishedMessagesBySeqNo.TryRemove(message.PublishSeqNo.Value, out _);
        }
    }
    
    /// <summary>
    /// All completions of the PendingMessage.TaskCompletionSource (after publishing) should live here
    /// </summary>
    /// <param name="message"></param>
    private void TryFinaliseMessage(PendingMessage message)
    {
        if (message.HasTimedOut)
        {
            TryRemoveMessage(message);
            message.TaskCompletionSource.TrySetException(
                new Exception($"Message publish {(message.Mandatory ? "and delivery " : "")}was not confirmed before timeout of {_sendTimeout} was reached."));
            return;
        }
        
        if (message.Mandatory)
        {
            switch (message.RoutingConfirmation)
            {
                case PendingMessage.RoutingConfirmationStatus.MessageReturned:
                    TryRemoveMessage(message);
                    message.TaskCompletionSource.TrySetException(
                        new Exception("Message was set to MandatoryRouting, but could not be routed."));
                    return;
                // in the case of a mandatory unroutable message, the basic.return is sent to the publishing client before the basic.nack.
                case PendingMessage.RoutingConfirmationStatus.AwaitingReturn:
                    break;
                default:
                    return;
            }
        }
        
        switch (message.PublishConfirmation)
        {
            case PendingMessage.PublishConfirmationStatus.Acked:
                break;
            case PendingMessage.PublishConfirmationStatus.Nacked:
                TryRemoveMessage(message);
                message.TaskCompletionSource.TrySetException(
                    new Exception($"Message publish failed - nacked by broker."));
                return;
            default:
                return;
        }
        
        TryRemoveMessage(message);

        message.TaskCompletionSource.TrySetResult();
    }
    
    private EventHandler<BasicReturnEventArgs> GetBasicReturnHandler()
    {
        return (sender, args) =>
        {
            if (args.BasicProperties.MessageId is null)
            {
                return;
            }
            
            if (!PublishedMessagesByMessageId.TryGetValue(args.BasicProperties.MessageId, out var message))
            {
                return;
            }
            
            message.RoutingConfirmation = PendingMessage.RoutingConfirmationStatus.MessageReturned;
            
            TryFinaliseMessage(message);
        };
    }

    private EventHandler<BasicAckEventArgs> GetBasicAckHandler()
    {
        return (sender, args) =>
        {
            if (args.Multiple)
            {
                var ackedMessages = PublishedMessagesBySeqNo
                    .Where(pair => pair.Key <= args.DeliveryTag)
                    .Select(p => p.Value).ToList();
                
                foreach (var message in ackedMessages)
                {
                    message.PublishConfirmation = PendingMessage.PublishConfirmationStatus.Acked;
                    TryFinaliseMessage(message);
                }
            }
            else
            {
                if (!PublishedMessagesBySeqNo.TryGetValue(args.DeliveryTag, out var message))
                {
                    return;
                }

                message.PublishConfirmation = PendingMessage.PublishConfirmationStatus.Acked;
                TryFinaliseMessage(message);
            }
        };
    }

    private EventHandler<BasicNackEventArgs> GetBasicNackHandler()
    {
        return (sender, args) =>
        {
            if (args.Multiple)
            {
                var nackedMessages = PublishedMessagesBySeqNo
                    .Where(pair => pair.Key <= args.DeliveryTag)
                    .Select(p => p.Value).ToList();
                
                foreach (var message in nackedMessages)
                {
                    message.PublishConfirmation = PendingMessage.PublishConfirmationStatus.Nacked;
                    TryFinaliseMessage(message);
                }
            }
            else
            {
                if (!PublishedMessagesBySeqNo.TryGetValue(args.DeliveryTag, out var message))
                {
                    return;
                }

                message.PublishConfirmation = PendingMessage.PublishConfirmationStatus.Nacked;
                TryFinaliseMessage(message);
            }
        };
    }

    private IModel GetOpenChannel()
    {
        if (Channel is null || !Channel.IsOpen)
        {
            Channel = _connection.CreateModel();
            Channel.ConfirmSelect();
            // No explicit unsubscription needed
            Channel.BasicReturn += GetBasicReturnHandler();
            Channel.BasicAcks += GetBasicAckHandler();
            Channel.BasicNacks += GetBasicNackHandler();
        }

        if (!ExchangeRedeclaredAt.HasValue || ExchangeRedeclaredAt.Value < DateTimeOffset.Now - RedeclareExchangeInterval)
        {
            Console.WriteLine($"Redeclaring exchange {ExchangeName}...");
            Channel.ExchangeDeclarePassive(ExchangeName);
            Console.WriteLine($"Finished redeclaring exchange {ExchangeName}...");
            if (Channel.IsOpen)
            {
                ExchangeRedeclaredAt = DateTimeOffset.Now;
            }
        }
        
        if (!Channel.IsOpen)
        {
            throw new Exception($"Failed to open channel for exchange {ExchangeName}. Reason: {Channel.CloseReason?.ReplyText}");
        }

        return Channel;
    }
    
    private long _semaphoreWaitTotal = 0;
    private long _dequeueWaitTotal = 0;
    private long _getOpenChannelWaitTotal = 0;
    private long _createBasicPropertiesTotal = 0;
    private long _basicPublishTotal = 0;
    private long _handleMessagePublishedTotal = 0;
    private long _outerLoopIterations = 0;
    private long _innerLoopIterations = 0;
    
    private async Task ReportDeltas(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(5000, token);
            Console.WriteLine("\n\n Reporting wait times:");
            Console.WriteLine("SemaphoreWait: " + (double)_semaphoreWaitTotal / _outerLoopIterations);
            Console.WriteLine("DequeueWait: " + (double)_dequeueWaitTotal / _innerLoopIterations);
            Console.WriteLine("GetOpenChannelWait: " + (double)_getOpenChannelWaitTotal / _innerLoopIterations);
            Console.WriteLine("CreateBasicPropertiesWait: " + (double)_createBasicPropertiesTotal / _innerLoopIterations);
            Console.WriteLine("BasicPublishWait: " + (double)_basicPublishTotal / _innerLoopIterations);
            Console.WriteLine("HandleMessagePublished: " + (double)_handleMessagePublishedTotal / _innerLoopIterations);
            Console.WriteLine("OuterLoopIterations: " + _outerLoopIterations);
            Console.WriteLine("InnerLoopIterations: " + _innerLoopIterations);
            Console.WriteLine("MessagesPendingPublish count: " + MessagesPendingPublish.Count);
            Console.WriteLine("PublishedMessagesByMessageId count: " + PublishedMessagesByMessageId.Count);
            Console.WriteLine("PublishedMessagesBySeqNo count: " + PublishedMessagesBySeqNo.Count);
        }
    }

    private async Task ProcessMessagesPendingPublish(CancellationToken cancellationToken)
    {

        var stopwatch = new Stopwatch();
        long delta = 0;
        stopwatch.Start();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            delta = stopwatch.ElapsedMilliseconds;
            await MessageEnqueuedSemaphore.WaitAsync(cancellationToken);
            _semaphoreWaitTotal += stopwatch.ElapsedMilliseconds - delta;
            

            while (!cancellationToken.IsCancellationRequested)
            {

                delta = stopwatch.ElapsedMilliseconds;
                
                if (!MessagesPendingPublish.TryDequeue(out var message))
                {
                    break;
                }

                _dequeueWaitTotal += stopwatch.ElapsedMilliseconds - delta;
                
                try
                {

                    delta = stopwatch.ElapsedMilliseconds;
                    var channel = GetOpenChannel();
                    _getOpenChannelWaitTotal += stopwatch.ElapsedMilliseconds - delta;
                    
                    message.PublishSeqNo = channel.NextPublishSeqNo;
                    
                    delta = stopwatch.ElapsedMilliseconds;
                    var props = channel.CreateBasicProperties();
                    _createBasicPropertiesTotal += stopwatch.ElapsedMilliseconds - delta;
                    
                    props.DeliveryMode = message.PersistentDelivery ? PersistentDeliveryMode : TransientDeliveryMode;
                    props.ContentType = message.ContentType;
                    props.Headers = message.Headers;
                    props.MessageId = message.MessageId;

                    delta = stopwatch.ElapsedMilliseconds;
                    channel.BasicPublish(
                        exchange: message.ExchangeName, 
                        routingKey: message.RoutingKey, 
                        mandatory: message.Mandatory,
                        basicProperties: props,
                        body: message.Body);
                    _basicPublishTotal += stopwatch.ElapsedMilliseconds - delta;

                    delta = stopwatch.ElapsedMilliseconds;
                    HandleMessagePublished(message.PublishSeqNo.Value, message.MessageId, message);
                    _handleMessagePublishedTotal += stopwatch.ElapsedMilliseconds - delta;
                }
                catch (Exception ex)
                {
                    message.TaskCompletionSource.TrySetException(ex);
                }

                _innerLoopIterations++;
            }

            _outerLoopIterations++;
        }
        stopwatch.Stop();
    }
}