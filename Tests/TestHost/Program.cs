using Odin.Logging;
using Odin.System;

namespace TestHost
{
    public class Program
    {
        public static int Main(string[] args)
        {
            AppBuilder builder = new AppBuilder(args);
            Outcome appBuild = builder.Build();
            if (!appBuild.Success)
            {
                ILoggerAdapter<AppBuilder>? logger = builder.TryGetLogger();
                if (logger != null)
                {
                    logger.LogCritical($"Application build failed: {appBuild.MessagesToString()}");
                }
                return -1;
            }

            try
            {
                builder.TryLog(LogLevel.Information, "Application built. Starting...");
                builder.App.Run();
                builder.TryLog(LogLevel.Information, "Application stopped...");
            }
            catch (Exception err)
            {
                builder.TryLog(LogLevel.Critical, $"Application run failed: {err.Message}");
                return -1;
            }
            
            return 0;
        }
    }
}