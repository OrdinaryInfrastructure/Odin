using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odin.Messaging.RabbitMq;

public class FakeRabbitConnectionService: IRabbitConnectionService
{

    public async Task SendAsync(string exchangeName, string routingKey, Dictionary<string, object> headers, string contentType, byte[] body, bool persistentDelivery = true,
        bool mandatoryRouting = false)
    {
        await Task.Delay(5);
    }

    public async Task<IRabbitConnectionService.Subscription> SubscribeToConsume(string queueName, bool autoAck, ushort prefetchCount = 200,
        TimeSpan? channelCheckPeriod = null)
    {
        await Task.Delay(5);

        return new IRabbitConnectionService.Subscription
        {
            Unsubscribe = () => Task.CompletedTask,
        };
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}