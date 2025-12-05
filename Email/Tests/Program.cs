using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Tests.Odin.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationOptions appOptions = new WebApplicationOptions()
            {
                Args = args
            };
            WebApplicationBuilder builder = WebApplication.CreateBuilder(appOptions);
            builder.Configuration.AddJsonFile("appSettings.json", false);
            builder.Configuration.AddUserSecrets<Program>();
            // builder.WebHost.ConfigureKestrel(opts => { opts.ListenLocalhost(Random.Shared.Next(5000, 5100)); });
            // builder.Services.AddLogger2();

            WebApplication app = builder.Build();

            app.RunAsync();
        }
    }
}