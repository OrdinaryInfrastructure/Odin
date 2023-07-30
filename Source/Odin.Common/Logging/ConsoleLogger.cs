using System;
using System.Text.Json;

namespace Odin.Logging
{
    /// <summary>
    /// Static utility methods for console logging in application Main and Startup before ILogger or ILoggerFactory are ready.
    /// </summary>
    public static class ConsoleLogger
    {
        /// <summary>
        /// Currently simply writes to the Console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        public static void Log(string message, Exception? err = null)
        {
            Console.WriteLine(message);
            if (err != null)
            {
                Log(err);
            }
        }
        
        /// <summary>
        /// Currently simply writes to the Console
        /// </summary>
        /// <param name="err"></param>
        public static void Log(Exception err)
        {
            if (err != null)
            {
                try
                {
                    Console.WriteLine(JsonSerializer.Serialize(err));
                }
                catch 
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

    }
}