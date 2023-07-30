using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

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
            builder.Configuration.AddJsonFile("appSettings.json", false);
            builder.Configuration.AddUserSecrets<TestProgram>();
            WebApplication app = builder.Build();
            app.RunAsync();
        }
    }
}