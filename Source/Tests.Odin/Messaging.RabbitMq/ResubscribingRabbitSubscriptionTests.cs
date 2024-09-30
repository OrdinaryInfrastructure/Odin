#nullable enable
using System;
using System.Text;
using System.Threading.Tasks;
using Odin.Messaging.RabbitMq;

namespace Tests.Odin.Messaging.RabbitMq;

public class ResubscribingRabbitSubscriptionTests()
{


    public async Task ResubscribingSubscription_Works()
    {
        try
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

            var factory = new ResubscribingRabbitSubscriptionFactory(
                connectionService,
                queueName,
                false,
                true,
                260,
                checkChannelPeriod: TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4)
            );

            var subscription = factory.Create();
            // RabbitConnectionServiceTests.TestMessage? mostRecent = null;

            subscription.OnFailure += ex =>
            {
                Console.WriteLine("OnFailure fired: " + ex);
                return Task.CompletedTask;
            };

            subscription.OnConsumed += async message =>
            {
                // var body = JsonSerializer.Deserialize<RabbitConnectionServiceTests.TestMessage>(message.Body);

                var body = Encoding.UTF8.GetString(message.Body);

                // Console.WriteLine("Received message " + body);

                // await Task.Delay(TimeSpan.FromSeconds(20));
                //
                // Console.WriteLine("StopConsuming");
                // subscription.StopConsuming();

                // await Task.Delay(TimeSpan.FromSeconds(3));
                //
                // Console.WriteLine("Closing channel");
                // await subscription.DisposeAsync();

                await Task.Delay(TimeSpan.FromMilliseconds(20));

                try
                {
                    message.AckNackCallbacks?.Ack.Invoke();
                    Console.WriteLine("Acked message " + body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to ack message " + body + "\nException: " + ex);
                }

                // await subscription.DisposeAsync();
                //
                // Console.WriteLine("Disposed subscription");


            };

            await Task.Delay(TimeSpan.FromSeconds(2));

            await subscription.StartConsuming();
            
            Console.WriteLine("Started consuming");

            await Task.Delay(TimeSpan.FromSeconds(120));

            Console.WriteLine("StopConsuming....");
            await subscription.StopConsuming();

            await Task.Delay(TimeSpan.FromSeconds(10));

            Console.WriteLine("Closing subscription");
            await subscription.DisposeAsync();

            // _ = Task.Run(async () =>
            // {
            //     while (true)
            //     {
            //         await Task.Delay(5000);
            //         Console.WriteLine("Most recent message: " + mostRecent);
            //     }
            // });

            await Task.Delay(TimeSpan.FromHours(3));

            // await subscription.DisposeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ResubscribingSubscription_Works failed: " + ex);
        }

    }
}