using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Odin.Messaging.RabbitMq;

namespace Tests.Odin.Messaging.RabbitMq;

public class RabbitConnectionServiceTests: IntegrationTest
{

    public record TestMessage
    {
        public required long MessageNumber { get; init; }
        public bool? Redelivered { get; set; }
        public required string ThreadIdentifier { get; init; }
    }

    [Test]
    [Ignore("So far, RabbitBox is tested manually only.")]
    public async Task Single_Message_Works()
    {
        var box = new RabbitConnectionService(new RabbitConnectionServiceSettings
        {
            Host = "localhost",
            VirtualHost = "odin-rabbitbox",
            Username = "rabbitbox-test",
            UserPassword = "rabbitbox-test-01",
            Port = 5672,
            ConnectionName = "RabbitBoxIntegTests",
            MaxChannels = 10,
            SendTimeoutMillis = 4000,
        });

        // foreach (var i in new[] { 1, 2 })
        // {
        //     var body = JsonSerializer.SerializeToUtf8Bytes(new { MyString = "ABCDE", MyLong = i }, new JsonSerializerOptions
        //     {
        //         WriteIndented = true,
        //     });
        //
        //     await box.SendAsync("test01", "", new Dictionary<string, object>(), "application/json", body, true, false);
        //     
        // }
        
        // string[] threadIdentifiers = ["Alpha", "Beta", "Gamma", "Delta"];

        List<string> threadIdentifiers = new();
        List<int> threadIdentifiersIndexes = new();

        HashSet<string> activeCallers = new();

        object activeCallersLock = new();



        for (var i = 0; i < 6000; i++)
        {
            threadIdentifiersIndexes.Add(i);
            threadIdentifiers.Add(Guid.NewGuid().ToString().Substring(0, 4));
        }

        threadIdentifiers[0] = "MONITOR";
        
        await Parallel.ForEachAsync(threadIdentifiersIndexes, new ParallelOptions
        {
            MaxDegreeOfParallelism = 6000,
        }, async (i, token) =>
        {

            var s = threadIdentifiers[i];

            // await Task.Delay(i * 15, token);

            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            long millis = 0;
            long sendAsyncTotalMillis = 0;
            long iteration = 0;
            while (!token.IsCancellationRequested)
            {
                // await Task.Delay(2500);
                iteration++;
                var body = JsonSerializer.SerializeToUtf8Bytes(new TestMessage
                {
                    MessageNumber = iteration,
                    Redelivered = null,
                    ThreadIdentifier = s,
                }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
                
                
                try
                {
                    millis = stopwatch.ElapsedMilliseconds;

                    var exchange = "test02-fanout"; // Random.Shared.NextDouble() > 0.5 ? "test02-fanout" : "test01";
                    
                    await box.SendAsync(exchange, "", new Dictionary<string, object>(), "application/json", body, true, false);
                    // TestContext.Progress.WriteLine($"Sent message {i}\n");
                    // Console.WriteLine($"Sent message {s} : {iteration}\n");

                    sendAsyncTotalMillis += stopwatch.ElapsedMilliseconds - millis;
                }
                catch (Exception e)
                {
                    // TestContext.Progress.WriteLine($"Failed to send message {i}:\n" + e);
                    Console.WriteLine($"Failed to send message {s} : {iteration}:\n" + e);
                }



                // if (iteration % 50 == 0)
                // {
                //     // Console.WriteLine($"\n--- Finished loop for caller {s} ---" +
                //     //                   $"active callers count: {activeCallersCount}\n");
                //     
                // }

                if (iteration % 50 == 0)
                {
                    lock (activeCallersLock)
                    {
                        activeCallers.Add(s);
                    }
                }

                if (s == "MONITOR" && iteration % 50 == 0)
                {
                    long? activeCallersCount = null;
                    lock (activeCallersLock)
                    {
                        activeCallersCount = activeCallers.Count;
                    }
                    Console.WriteLine($"\nReporting for caller {s}:" +
                                      $"\nIteration: {iteration}" +
                                      $"\nMean SendAsync wait: {(double)sendAsyncTotalMillis / iteration}" +
                                      $"\nActive callers count: {(activeCallersCount.HasValue ? activeCallersCount.Value : "unknown")}" +
                                      $"\nEnd reporting for caller {s}");
                }
                
                // await Task.Delay(5000, token);
                
            }
        });
    }

    public async Task QueueSubscription_Works()
    {
        var box = new RabbitConnectionService(new RabbitConnectionServiceSettings
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

        Func<Task> cancelSubscription = () => {Console.WriteLine("No-op action invoked");
            return Task.CompletedTask;
        };
        try
        {
            cancelSubscription = await box.SubscribeToConsume(queueName,
                ex =>
                {
                    Console.WriteLine("OnFailure callback called. Exception: " + ex);
                    return Task.CompletedTask;
                },
                async message =>
                {
                    string str = "no body";
                    try
                    {
                        str = Encoding.UTF8.GetString(message.Body);
                        var body = JsonSerializer.Deserialize<TestMessage>(str);
                        // var body = JsonSerializer.Deserialize<TestMessage>(message.Body);
                        body.Redelivered = message.IsRedelivered;
                        // Console.WriteLine("OnConsume callback called. Waiting 4 seconds to ack. Message body: " + body);
                        await Task.Delay(1);
                        bool nackMessage = Random.Shared.NextDouble() > 0.5;
                        // bool nackMessage = false;
                        if (message.Ack != null && !nackMessage)
                        {
                            message.Ack();
                        }
                        else if (message.Nack != null)
                        {
                            message.Nack(true);
                        }

                        // Console.WriteLine($"{(nackMessage ? "Nacked" : "Acked")} message " + body);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to handle consumed message. Body: " + str + "\nException: " + e);
                        if (message.Nack != null)
                        {
                            message.Nack(false);
                        }
                    }
                },
                false,
                200,
                TimeSpan.FromSeconds(5));

            Console.WriteLine($"Subscribed to queue {queueName}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to subscribe to queue {queueName}. Exception: " + ex);
        }
        
        var queueName2 = "my-queue.q";

        Func<Task> cancelSubscription2 = () => {Console.WriteLine("No-op action invoked");
            return Task.CompletedTask;
        };
        try
        {
            cancelSubscription2 = await box.SubscribeToConsume(queueName2,
                ex =>
                {
                    Console.WriteLine("OnFailure callback called. Exception: " + ex);
                    return Task.CompletedTask;
                },
                async message =>
                {
                    string str = "no body";
                    try
                    {
                        str = Encoding.UTF8.GetString(message.Body);
                        var body = JsonSerializer.Deserialize<TestMessage>(str);
                        // var body = JsonSerializer.Deserialize<TestMessage>(message.Body);
                        body.Redelivered = message.IsRedelivered;
                        // Console.WriteLine("OnConsume callback called. Waiting 4 seconds to ack. Message body: " + body);
                        await Task.Delay(1);
                        bool nackMessage = Random.Shared.NextDouble() > 0.5;
                        // bool nackMessage = false;
                        if (message.Ack != null && !nackMessage)
                        {
                            message.Ack();
                        }
                        else if (message.Nack != null)
                        {
                            message.Nack(true);
                        }

                        // Console.WriteLine($"{(nackMessage ? "Nacked" : "Acked")} message " + body);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to handle consumed message. Body: " + str + "\nException: " + e);
                        if (message.Nack != null)
                        {
                            message.Nack(false);
                        }
                    }
                },
                false,
                200,
                TimeSpan.FromSeconds(5));

            Console.WriteLine($"Subscribed to queue {queueName2}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to subscribe to queue {queueName2}. Exception: " + ex);
        }


        await Task.Delay(TimeSpan.FromSeconds(3600));

        try
        {
            await cancelSubscription();
            await cancelSubscription2();
            Console.WriteLine($"Unsubscribed from queue {queueName2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to unsubscribe from queue {queueName2}. Exception: " + ex);
        }

    }
    
    
    
}