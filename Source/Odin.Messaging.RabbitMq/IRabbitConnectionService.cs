using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odin.Messaging.RabbitMq;

public interface IRabbitConnectionService: IAsyncDisposable
{
    /// <summary>
    /// Throws if message cannot be published (guaranteed publishing)
    /// </summary>
    /// <param name="exchangeName"></param>
    /// <param name="routingKey"></param>
    /// <param name="headers"></param>
    /// <param name="body"></param>
    /// <param name="mandatoryRouting">If true, throws if the message cannot be delivered (cannot be routed to any queue).</param>
    /// <returns></returns>
    public Task SendAsync(string exchangeName, string routingKey, Dictionary<string, object> headers, string contentType, byte[] body, bool persistentDelivery = true, bool mandatoryRouting = false);

    public record ConsumedMessage
    {
        public required byte[] Body { get; init; }
        public required DateTimeOffset ConsumedAt { get; init; }
        public required bool IsRedelivered { get; init; }
        /// <summary>
        /// Exchange which routed this message.
        /// </summary>
        public required string Exchange { get; init; }
        
        public required string RoutingKey { get; init; }
        /// <summary>
        /// Consumer (i.e. subscription) identifier
        /// </summary>
        public required string ConsumerTag { get; init; }
        
        // Message properties
        
        public string? CorrelationId { get; init; }
        
        public Dictionary<string, object> Headers { get; init; } = new();
        public string? MessageId { get; init; }
        public string? ReplyTo { get; init; }
        public string? Type { get; init; }
        public DateTimeOffset? Timestamp { get; init; }
        
        /// <summary>
        /// If the subscription is AutoAck = false, these callbacks are non-null and calling code MUST call either Ack or Nack after receiving the message.
        /// If the subscription is AutoAck = true, these callbacks are null.
        /// </summary>
        public Action? Ack { get; init; }
        /// <summary>
        /// The bool argument of this callback is whether the nacked message should be re-queued.
        /// </summary>
        public Action<bool>? Nack { get; init; }
    }

    public class Subscription
    {
        /// <summary>
        /// Called for each consumed message. Exceptions thrown by the given callback are ignored.
        /// </summary>
        public event Func<ConsumedMessage, Task>? OnConsumed;

        internal void RaiseOnConsumed(ConsumedMessage message)
        {
            _ = OnConsumed?.Invoke(message);
        }
        
        /// <summary>
        /// Called if the connection breaks. Exceptions thrown by the given callback are ignored. May be called multiple times for the same failure.
        /// </summary>
        public event Func<Exception, Task>? OnFailure;

        internal void RaiseOnFailure(Exception exception)
        {
            _ = OnFailure?.Invoke(exception);
        }
        
        /// <summary>
        /// An async callback that, when invoked, cancels the subscription. When this is done, the OnFailure event is fired twice, which should be ignored.
        /// </summary>
        public required Func<Task> Unsubscribe { get; init; }
    }

    /// <summary>
    /// Subscribes to a RabbitMQ Queue.
    /// Throws if a subscription already exists for the queueName, even if this subscription has failed. The callback to unsubscribe must be awaited, and then
    /// re-subscription may be attempted.
    /// Throws if the initial connection, channel and consumer cannot be established.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="autoAck">Whether the channel and consumer are in automatic acknowledgement mode. If false, the channel and consumer are in manual acknowledgement mode
    /// and the subscribing code MUST call either Ack() or Nack() on the ConsumedMessage after receiving it.</param>
    /// <param name="prefetchCount">When AutoAck = false, the maximum number of messages that RabbitMQ will allow to be in-flight to the consumer (i.e. not yet acknowledged).</param>
    /// <param name="channelCheckPeriod">The period with which to periodically check that the channel is still open. Default 60 seconds. If the channel is closed, calls onFailure.</param>
    public Task<Subscription> SubscribeToConsume(string queueName, bool autoAck, ushort prefetchCount = 200, TimeSpan? channelCheckPeriod = null);
}