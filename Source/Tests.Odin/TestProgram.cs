using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tests.Odin.Messaging.RabbitMq;

namespace Tests.Odin
{
    public class TestProgram
    {
        public static async Task Main(string[] args)
        {
            WebApplicationOptions appOptions = new WebApplicationOptions()
            {
                Args = args
            };
            WebApplicationBuilder builder = WebApplication.CreateBuilder(appOptions);
            builder.Configuration.AddJsonFile("appSettings.json", false);
            builder.Configuration.AddUserSecrets<TestProgram>();

            builder.WebHost.ConfigureKestrel(opts =>
            {
                opts.ListenLocalhost(Random.Shared.Next(5000, 5100));
            });
            
            builder.Services.AddLoggerAdapter();
            
            WebApplication app = builder.Build();

            var runTask = app.RunAsync();

            var stoppingCts = app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;

            // For manual testing of RabbitBox

            var tests = new RabbitConnectionServiceTests();

            // await tests.Publish_Works(stoppingCts); 
            
            // await tests.QueueSubscription_Works(stoppingCts);

            var resubscriberTests = new ResubscribingRabbitSubscriptionTests();

            // await resubscriberTests.ResubscribingSubscription_Works(stoppingCts);

            // var clientInvestigations = new RabbitClientInvestigations();

            // _ = clientInvestigations.CreateChannel_Is_ThreadSafe();

            // Thread.Sleep(TimeSpan.FromHours(13));

            await runTask;

        }
    }
}