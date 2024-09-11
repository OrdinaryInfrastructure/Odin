#nullable enable
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
            SendTimeoutMillis = 1000,
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



        for (var i = 0; i < 1; i++)
        {
            threadIdentifiersIndexes.Add(i);
            threadIdentifiers.Add(Guid.NewGuid().ToString().Substring(0, 4));
        }

        threadIdentifiers[0] = "MONITOR";
        
        await Parallel.ForEachAsync(threadIdentifiersIndexes, new ParallelOptions
        {
            MaxDegreeOfParallelism = 600,
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
                await Task.Delay(2500);
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

        IRabbitConnectionService.Subscription? subscription = null;
        try
        {
            TestMessage? consumedMessage = null;
            
            subscription = await box.SubscribeToConsume(queueName, false, true, 200, TimeSpan.FromSeconds(5));
            
            subscription.OnConsumed += async message =>
            {
                string str = "no body";
                try
                {
                    str = Encoding.UTF8.GetString(message.Body);
                    var body = JsonSerializer.Deserialize<TestMessage>(str)!;
                    consumedMessage = body;
                    // var body = JsonSerializer.Deserialize<TestMessage>(message.Body);
                    body.Redelivered = message.IsRedelivered;
                    // Console.WriteLine("OnConsume callback called. Waiting 4 seconds to ack. Message body: " + body);
                    await Task.Delay(30);
                    // bool nackMessage = Random.Shared.NextDouble() > 0.5;
                    bool nackMessage = false;

                    if (message.AckNackCallbacks is not null)
                    {
                        if (nackMessage)
                        {
                            message.AckNackCallbacks.Nack(true);
                        }
                        else
                        {
                            message.AckNackCallbacks.Ack();
                        }
                    }
                    
                    Console.WriteLine($"{(nackMessage ? "Nacked" : "Acked")} message " + body);

                    // if (body.MessageNumber % 10 == 0)
                    // {
                    //     throw new Exception("Sneed 10");
                    // }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to handle consumed message. Body: " + str + "\nException: " + e);
                    if (message.AckNackCallbacks != null)
                    {
                        message.AckNackCallbacks.Nack(false);
                    }
                }
            };

            subscription.OnFailure += ex =>
            {
                Console.WriteLine("OnFailure callback called. Exception: " + ex);
                return Task.CompletedTask;
            };
            
            // _ = Task.Run(async () =>
            // {
            //     while (true)
            //     {
            //         await Task.Delay(1000);
            //         Console.WriteLine("Most recently consumed message: " + consumedMessage);
            //     }
            // });

            await Task.Delay(TimeSpan.FromSeconds(20));

            Console.WriteLine($"Will start consuming queue {queueName} with subscription1.");
            
            await subscription.StartConsuming();

            Console.WriteLine($"Started consuming queue {queueName} with subscription1.");
            
            await Task.Delay(TimeSpan.FromSeconds(300));
            
            await subscription.StopConsuming();
            
            Console.WriteLine($"Stopped consuming queue {queueName}");

            
            await Task.Delay(TimeSpan.FromSeconds(1));

            await subscription.CloseChannel();
            
            Console.WriteLine($"Closed channel for queue {queueName}");


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something failed testing queue {queueName}. Exception: " + ex);
        }
        

        await Task.Delay(TimeSpan.FromHours(12));

        try
        {
            if (subscription is not null)
            {
                await subscription.CloseChannel();
                Console.WriteLine($"Unsubscribed from queue {queueName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to unsubscribe from queue {queueName}. Exception: " + ex);
        }

    }
    
    
    
}