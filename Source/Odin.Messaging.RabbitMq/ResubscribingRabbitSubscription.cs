using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Odin.Logging;

namespace Odin.Messaging.RabbitMq;

public class ResubscribingRabbitSubscription: IAsyncDisposable
{
    
    private ConcurrentDictionary<long, IRabbitConnectionService.Subscription> _subscriptions = new();
    
    /// <summary>
    /// Raised for messages consumed, as per Subscription.
    /// </summary>
    public event Func<IRabbitConnectionService.ConsumedMessage, Task>? OnConsumed;

    /// <summary>
    /// OnFailure is triggered when the Channel is closed, and few other failure scenarios.
    /// </summary>
    public event Func<Exception, Task>? OnFailure;

    private CancellationTokenSource _cancellationTokenSource = new();
    private SemaphoreSlim _createSubscriptionSemaphore = new SemaphoreSlim(0);

    private TimeSpan _attemptReconnectPeriod;
    private string _queueName;
    private bool _autoAck;
    private ushort _prefetchCount;
    private TimeSpan _checkChannelPeriod;

    private IRabbitConnectionService _connectionService;
    private ILoggerAdapter<ResubscribingRabbitSubscription> _logger;

    public ResubscribingRabbitSubscription(
        IRabbitConnectionService connectionService, 
        ILoggerAdapter<ResubscribingRabbitSubscription> logger,
        string queuename,
        bool autoAck,
        ushort prefetchCount = 200,
        TimeSpan? checkChannelPeriod = null,
        TimeSpan? attemptReconnectPeriod = null
        )
    {
        _connectionService = connectionService;
        _logger = logger;
        _queueName = queuename;
        _autoAck = autoAck;
        _prefetchCount = prefetchCount;
        _checkChannelPeriod = checkChannelPeriod ?? TimeSpan.FromSeconds(5);
        _attemptReconnectPeriod = attemptReconnectPeriod ?? TimeSpan.FromSeconds(30);
        
        _ = TryCreateSubscription(_cancellationTokenSource.Token);

        _createSubscriptionSemaphore.Release();
    }

    private SemaphoreSlim _failureHandlingSemaphore = new SemaphoreSlim(1);
    
    private long _currentSubscriptionNumber = 0;

    private bool _consumerExplicitlyCancelled = false;
    
    private async Task HandleFailure(long subscriptionNumber, Exception exception)
    {
        if (_cancellationTokenSource.Token.IsCancellationRequested)
        {
            return;
        }

        if (_consumerExplicitlyCancelled && exception is IRabbitConnectionService.ConsumerCancelledException consumerCancelledException)
        {
            return;
        }
        
        _ = OnFailure?.Invoke(exception);
        
        await _failureHandlingSemaphore.WaitAsync();
            
        try
        {
            var willCancelSubscription = subscriptionNumber == _currentSubscriptionNumber;

            _logger.LogError($"Subscription {subscriptionNumber} to queue {_queueName} failed. " +
                             $"{(willCancelSubscription ? "Will attempt re-subscription." : "Ignoring, since this subscription has already failed.")}", exception);
            
            if (!willCancelSubscription)
            {
                return;
            }

            if (!_subscriptions.TryRemove(subscriptionNumber, out var subscription))
            {
                return;
            }

            subscription.OnConsumed -= RaiseConsumed;
            await subscription.CloseChannel();

            _currentSubscriptionNumber++;
            
            _createSubscriptionSemaphore.Release();

        }
        finally
        {
            _failureHandlingSemaphore.Release();
        }

    }

    /// <summary>
    /// Stops consuming new messages, but messages already consumed can still be acked or nacked.
    /// Shutdown procedure is to first call this, then ack (or nack) all outstanding messages, then await DisposeAsync().
    /// </summary>
    public void StopConsuming()
    {
        _consumerExplicitlyCancelled = true;
        foreach (var subscription in _subscriptions.Values)
        {
            subscription.StopConsuming();
        }
    }
    
    private async Task TryCreateSubscription(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await _createSubscriptionSemaphore.WaitAsync();
            

            while (!token.IsCancellationRequested)
            {

                try
                {
                    await _failureHandlingSemaphore.WaitAsync();
                    

                    if (_subscriptions.ContainsKey(_currentSubscriptionNumber))
                    {
                        break;
                    }
                    
                    var newSubscription = await _connectionService.SubscribeToConsume(_queueName, _autoAck, _prefetchCount, _checkChannelPeriod);
                    _subscriptions.TryAdd(_currentSubscriptionNumber, newSubscription);
                    newSubscription.OnConsumed += RaiseConsumed;
                    var subscriptionNumberCopy = _currentSubscriptionNumber;
                    newSubscription.OnFailure += ex =>
                    {
                        _ = HandleFailure(subscriptionNumberCopy, ex);
                        return Task.CompletedTask;
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to subscribe to queue {_queueName}.", ex);
                    OnFailure?.Invoke(ex);
                }
                finally
                {
                    _failureHandlingSemaphore.Release();
                }

                await Task.Delay(_attemptReconnectPeriod, token);
            }
        }
        
    }

    private Task RaiseConsumed(IRabbitConnectionService.ConsumedMessage message)
    {
        _ = OnConsumed?.Invoke(message);
        return Task.CompletedTask;
    }
    
    
    
    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        foreach (var s in _subscriptions.Values)
        {
            try
            {
                await s.CloseChannel();
            }
            catch
            {
                
            }
        }
    }
}