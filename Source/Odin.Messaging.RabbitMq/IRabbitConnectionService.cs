using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odin.Messaging.RabbitMq;

/// <summary>
/// Manages a single RabbitMQ Connection over which can be created
/// RabbitMQ Channels (up to a configurable maximum, currently 10) as needed.
/// A separate Channel is automatically created for each distinct Queue name subscribed to,
/// and for each distinct Exchange published to.
/// </summary>
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
        
        /// <summary>
        /// Returns true if this message is not being delivered for the first time.
        /// </summary>
        public required bool IsRedelivered { get; init; }
        
        /// <summary>
        /// Exchange which routed this message.
        /// </summary>
        public required string Exchange { get; init; }
        
        /// <summary>
        /// NB property of a RabbitMQ message
        /// </summary>
        public required string RoutingKey { get; init; }
        
        /// <summary>
        /// Consumer (i.e. subscription) identifier. Another RabbitMQ thing.
        /// </summary>
        public required string ConsumerTag { get; init; }
        
        // Message properties
        
        
        /// <summary>
        /// Optional property that might have been set by the publisher of the message
        /// </summary>
        public string? CorrelationId { get; init; }
        
        /// <summary>
        /// Optional property that might have been set by the publisher of the message
        /// </summary>
        public Dictionary<string, object> Headers { get; init; } = new();
        /// <summary>
        /// Optional property that might have been set by the publisher of the message
        /// </summary>
        public string? MessageId { get; init; }
        /// <summary>
        /// Optional property that might have been set by the publisher of the message
        /// </summary>
        public string? ReplyTo { get; init; }
        /// <summary>
        /// Optional property that might have been set by the publisher of the message
        /// </summary>
        public string? Type { get; init; }
        /// <summary>
        /// Optional property that might have been set by the publisher of the message
        /// </summary>
        public DateTimeOffset? Timestamp { get; init; }
        
        /// <summary>
        /// Ack - Acknowledge. If the subscription is AutoAck = false, these callbacks are non-null and calling code MUST call either Ack or Nack after receiving the message.
        /// If the subscription is AutoAck = true, these callbacks are null.
        /// </summary>
        public Action? Ack { get; init; }
        
        /// <summary>
        /// Nack - Not acknowledge. The bool argument of this callback is whether the nacked message should be re-queued.
        /// </summary>
        public Action<bool>? Nack { get; init; }
    }

    /// <summary>
    /// Represents a RabbitMQ Consumer that has a channel to itself.
    /// </summary>
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