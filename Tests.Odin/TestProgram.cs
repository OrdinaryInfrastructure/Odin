using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Odin
{
    public class TestProgram
    {
        private static string FindContentRootFromBaseDirectory(string baseDirectory)
        {
            // Start from the current domain base directory
            var currentDirectory = new DirectoryInfo(baseDirectory);
           
            // Walk up the directory tree until we find the solution file or project directory
            while (currentDirectory != null)
            {
                // Look for solution file in current directory
                if (currentDirectory.GetFiles("Odin.slnx").Any())
                {
                    return currentDirectory.FullName;
                }
                
                // Look for Tests.Odin directory (if we're not already in it)
                var testsOdinDir = currentDirectory.GetDirectories("Tests.Odin").FirstOrDefault();
                if (testsOdinDir != null)
                {
                    return testsOdinDir.FullName;
                }
                
                // If we're in the Tests.Odin directory, check if parent has solution file
                if (currentDirectory.Name == "Tests.Odin")
                {
                    var parentDir = currentDirectory.Parent;
                    if (parentDir != null && parentDir.GetFiles("Odin.slnx").Any())
                    {
                        return currentDirectory.FullName;
                    }
                }
                currentDirectory = currentDirectory.Parent;
            }
            return null;
        }

        public static async Task Main(string[] args)
        {
            WebApplicationOptions appOptions = new WebApplicationOptions()
            {
                Args = args,
                ContentRootPath = FindContentRootFromBaseDirectory(AppDomain.CurrentDomain.BaseDirectory)
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