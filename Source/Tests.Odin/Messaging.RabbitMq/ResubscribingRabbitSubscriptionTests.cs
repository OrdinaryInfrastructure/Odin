#nullable enable
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Odin.Logging;
using Odin.Messaging.RabbitMq;

namespace Tests.Odin.Messaging.RabbitMq;

public class ResubscribingRabbitSubscriptionTests
{


    public async Task ResubscribingSubscription_Works()
    {
        var connectionService = new RabbitConnectionService(new RabbitConnectionServiceSettings
        {
            Host = "localhost",
            VirtualHost = "odin-rabbitbox",
            Username = "rabbitbox-test",
            UserPassword = "rabbitbox-test-01",
            Port = 5672,
            ConnectionName = "RabbitBoxIntegTests",
            MaxChannels = 10,
            SendTimeoutMillis = 5000,
        });
        
        var queueName = "max-length-test-01";

        var subscription = new ResubscribingRabbitSubscription(
            connectionService, 
            new NullLogger<ResubscribingRabbitSubscription>(), 
            queueName, 
            false,
            200,
            checkChannelPeriod: TimeSpan.FromSeconds(2), 
            TimeSpan.FromSeconds(5));

        RabbitConnectionServiceTests.TestMessage? mostRecent = null;

        subscription.OnConsumed += async message =>
        {
            var body = JsonSerializer.Deserialize<RabbitConnectionServiceTests.TestMessage>(message.Body);

            await Task.Delay(1);
            
            // Console.WriteLine($"Consumed message from resubscriber: " + body);

            if (body?.MessageNumber % 100 == 0)
            {
                mostRecent = body;
            }
            
            message.Ack?.Invoke();
            
        };

        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(5000);
                Console.WriteLine("Most recent message: " + mostRecent);
            }
        });

        await Task.Delay(TimeSpan.FromHours(3));

        await subscription.DisposeAsync();

    }
}