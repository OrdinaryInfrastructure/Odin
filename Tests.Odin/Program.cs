using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Tests.Odin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string currentDir = Directory.GetCurrentDirectory();
            WebApplicationOptions appOptions = new WebApplicationOptions()
            {
                Args = args,
                WebRootPath = currentDir,
                ContentRootPath = currentDir
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