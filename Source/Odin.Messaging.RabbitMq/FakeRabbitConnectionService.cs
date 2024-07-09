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

    public async Task<Func<Task>> SubscribeToConsume(string queueName, Func<Exception, Task> onFailure, Func<IRabbitConnectionService.ConsumedMessage, Task> onConsume, bool autoAck, ushort prefetchCount = 200,
        TimeSpan? channelCheckPeriod = null)
    {
        await Task.Delay(5);

        return () => Task.CompletedTask;
    }
}