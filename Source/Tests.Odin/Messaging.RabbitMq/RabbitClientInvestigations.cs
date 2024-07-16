#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Odin.Messaging.RabbitMq;
using RabbitMQ.Client;

namespace Tests.Odin.Messaging.RabbitMq;

public class RabbitClientInvestigations
{


    public async Task CreateChannel_Is_ThreadSafe()
    {

        var settings = new RabbitConnectionServiceSettings
        {
            Host = "localhost",
            VirtualHost = "odin-rabbitbox",
            Username = "rabbitbox-test",
            UserPassword = "rabbitbox-test-01",
            Port = 5672,
            ConnectionName = "RabbitBoxIntegTests",
            MaxChannels = 10,
            SendTimeoutMillis = 1000,
        };
        
        var factory = new ConnectionFactory()
        {
            AutomaticRecoveryEnabled = true,
            // DispatchConsumersAsync = false,
            // ConsumerDispatchConcurrency = 0,
            HostName = settings.Host,
            Port = settings.Port,
            TopologyRecoveryEnabled = false,
            // ClientProperties = null,
            UserName = settings.Username,
            Password = settings.UserPassword,
            VirtualHost = settings.VirtualHost,
            ClientProvidedName = settings.ConnectionName,
        };

        var connection = factory.CreateConnection();

        int[] threads = [0, 1, 2, 3, 4, 5];

        var channels = new List<IModel?>();

        foreach (var t in threads)
        {
            channels.Add(null);
        }

        _ = Parallel.ForEachAsync(threads, new ParallelOptions{MaxDegreeOfParallelism = threads.Length}, async (i, token) =>
        {
            int loopCounter = 0;
            int reportEvery = 10;
            while (!token.IsCancellationRequested)
            {
                var channel = channels[i];

                if (channel is null)
                {
                    try
                    {
                        if (i == 0 && loopCounter % reportEvery == 0)
                        {
                            Console.WriteLine($"Thread 0 creating channel {loopCounter}");
                        }
                        var newChannel = connection.CreateModel();
                        if (newChannel is null)
                        {
                            throw new Exception("newChannel is null");
                        }
                        if (i == 0 && loopCounter % reportEvery == 0)
                        {
                            Console.WriteLine($"Thread 0 created channel {loopCounter}");
                        }
                        channels[i] = newChannel;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Thread {i} failed to open channel {loopCounter}: " + ex);
                    }
                }
                else
                {
                    try
                    {
                        if (i == 0 && loopCounter-1 % reportEvery == 0)
                        {
                            Console.WriteLine($"Thread 0 closing channel {loopCounter}");
                        }
                        channel.Close();
                        if (i == 0 && loopCounter-1 % reportEvery == 0)
                        {
                            Console.WriteLine($"Thread 0 closed channel {loopCounter}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Thread {i} failed to close channel {loopCounter}: " + ex);
                    }
                    channels[i] = null;
                }

                await Task.Delay(1, token);
                loopCounter++;
            }
        });

        await Task.Delay(TimeSpan.FromMinutes(10));
    }
}