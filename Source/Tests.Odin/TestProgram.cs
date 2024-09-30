using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.Odin.Messaging.RabbitMq;

namespace Tests.Odin
{
    public class TestProgram
    {
        public static void Main(string[] args)
        {
            WebApplicationOptions appOptions = new WebApplicationOptions()
            {
                Args = args
            };
            WebApplicationBuilder builder = WebApplication.CreateBuilder(appOptions);
            // builder.Configuration.AddJsonFile("appSettings.json", false);
            builder.Configuration.AddUserSecrets<TestProgram>();
            
            builder.Services.AddLoggerAdapter();

            
            
            WebApplication app = builder.Build();
            // app.RunAsync();

            // For manual testing of RabbitBox
            
            var tests = new RabbitConnectionServiceTests();
            // _ = tests.Single_Message_Works(); 
            // _ = tests.QueueSubscription_Works();

            var resubscriberTests = new ResubscribingRabbitSubscriptionTests();
            
            _ = resubscriberTests.ResubscribingSubscription_Works();

            // var clientInvestigations = new RabbitClientInvestigations();

            // _ = clientInvestigations.CreateChannel_Is_ThreadSafe();
            
            Thread.Sleep(TimeSpan.FromHours(13));
            
        }
    }
}