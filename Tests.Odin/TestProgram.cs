using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            Task runTask = app.RunAsync();

            await runTask;
            
            // CancellationToken stoppingCts = app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;

            // For manual testing of RabbitBox

            // RabbitConnectionServiceTests tests = new RabbitConnectionServiceTests();

            // await tests.Publish_Works(stoppingCts); 
            
            // await tests.QueueSubscription_Works(stoppingCts);

            // ResubscribingRabbitSubscriptionTests resubscriberTests = new ResubscribingRabbitSubscriptionTests();

            // await resubscriberTests.ResubscribingSubscription_Works(stoppingCts);

            // var clientInvestigations = new RabbitClientInvestigations();

            // _ = clientInvestigations.CreateChannel_Is_ThreadSafe();

            // Thread.Sleep(TimeSpan.FromHours(13));

        }
    }
}