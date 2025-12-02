using Microsoft.AspNetCore.Authorization;
using Odin.BackgroundProcessing;
using Odin.Email;
using Odin.Logging;
using Odin.System;
using TestHost.Authorization;

namespace TestHost;

public class AppBuilder
{
    private string[] _args;

    public AppBuilder(string[] args)
    {
        _args = args;
    }

    public WebApplication App { get; private set; } = null!;
    public WebApplicationBuilder Builder { get; private set; } = null!;

    public bool IsBuilt { get; private set; } = false;

    public ILoggerAdapter<AppBuilder>? TryGetLogger()
    {
        if (App == null!) return null;
        try
        {
            ILoggerAdapter<AppBuilder>? logger = App?.Services?.GetService<ILoggerAdapter<AppBuilder>>();
            if (logger != null)
            {
                return logger;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public void TryLog(LogLevel level, string message)
    {
        ILoggerAdapter<AppBuilder>? logger = TryGetLogger();
        if (logger != null)
        {
            logger.Log(level, message);
        }
    }

    public Outcome Build()
    {
        IsBuilt = false;

        WebApplicationOptions appOptions = new WebApplicationOptions()
        {
            ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
            Args = _args
        };
        Builder = WebApplication.CreateBuilder(appOptions);
        Builder.Host.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);

        /////////////////////////////////////////////////////////////
        // Configuration
        /////////////////////////////////////////////////////////////
        Builder.Configuration.AddJsonFile("appSettings.json");
        Builder.Configuration.AddUserSecrets<Program>();

        /////////////////////////////////////////////////////////////
        // Logging and Telemetry
        /////////////////////////////////////////////////////////////
        Builder.Logging.AddConfiguration(Builder.Configuration.GetSection("Logging"));
        Builder.Logging.AddConsole();
        if (Builder.Environment.IsDevelopment())
        {
            Builder.Logging.AddDebug();
        }

        Builder.Services.AddAuthentication();
        Builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.AllowAnonymous,
                policy => { policy.Requirements.Add(new AlwaysAllowRequirement()); }
            );
            options.DefaultPolicy = options.GetPolicy(AuthPolicies.AllowAnonymous)!;
        });
        Builder.Services.AddSingleton<IAuthorizationHandler, AlwaysAllowHandler>();
        Builder.Services.AddLoggerAdapter();
        Builder.Services.AddBackgroundProcessing(Builder.Configuration);
        Builder.Services.AddEmailSending(Builder.Configuration);

        // App specific
        Builder.Services.AddTransient<ITestService, TestService>();


        /////////////////////////////////////////////////////////////
        // ASP.NET Related
        /////////////////////////////////////////////////////////////
        Builder.Services.AddControllers();
        Builder.Services.AddRazorPages();

        /////////////////////////////////////////////////////////////
        // Build the application
        /////////////////////////////////////////////////////////////
        App = Builder.Build();

        // Get our logger for remainder of startup...
        ILoggerAdapter<AppBuilder>? logger = TryGetLogger();
        if (logger == null)
        {
            throw new Exception("Startup failed to initialise an ILoggerAdapter");
        }

        logger.LogInformation($"{nameof(Build)}: Build succeeded.");

        ///////////////////////////////////
        // BackgroundProcessing and Hangfire
        /////////////////////////////////

        logger.LogInformation($"{nameof(Build)}: Setting up Hangfire...");

        App.UseBackgroundProcessing(App.Services);
        
        IBackgroundProcessor backgroundProcessor = App.Services.GetRequiredService<IBackgroundProcessor>();
        backgroundProcessor.AddOrUpdateRecurringJob<ITestService>(bm => bm.HelloWorld(), "HelloWorld",
            "1 12 * * *", TimeZoneInfo.Local);
        backgroundProcessor.ScheduleJob<ITestService>(bm => bm.HelloWorld(), TimeSpan.FromMinutes(10));

        App.UseDeveloperExceptionPage();
        App.UseStaticFiles();
        App.UseRouting();
        App.MapControllers();
        App.MapRazorPages();


        IsBuilt = true;
        return Outcome.Succeed();
    }
}