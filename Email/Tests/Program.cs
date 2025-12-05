using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Tests.Odin.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string baseDir = AppContext.BaseDirectory;
            WebApplicationOptions appOptions = new WebApplicationOptions()
            {
                Args = args,
                ContentRootPath = baseDir
            };
            WebApplicationBuilder builder = WebApplication.CreateBuilder(appOptions);
            builder.Configuration.AddJsonFile("appSettings.json", false);
            builder.Configuration.AddUserSecrets<Program>();
            WebApplication app = builder.Build();
            app.RunAsync();
        }
    }
}