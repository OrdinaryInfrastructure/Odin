using System;
using Microsoft.Extensions.Logging;

namespace Odin.Logging
{
    /// <summary>
    /// Logger that does not do anything.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class NullLogger<T> : ILoggerAdapter<T>
    {

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="err"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string? message, Exception? err)
        {
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="err"></param>
        public void Log(LogLevel level, Exception err)
        {
            Log(level, err.Message, err);
        }
        
        /// <summary>
        /// Structured Log Message. Object array contains a list of values to populate
        /// the logging keys e.g. {ExampleKey} that are included in the log. 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogStructured(
            LogLevel logLevel,
            Exception? exception,
            string? message,
            params object?[] args)
        {
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="argsToLogAsJson"></param>
        public void LogToJson(LogLevel level, params object[] argsToLogAsJson)
        {
        }

        /// <summary>
        /// LogTrace
        /// </summary>
        /// <param name="message"></param>
        public void LogTrace(string message)
        {
        }

        /// <summary>
        /// LogDebug
        /// </summary>
        /// <param name="message"></param>
        public void LogDebug(string message)
        {
        }

        /// <summary>
        /// LogInformation
        /// </summary>
        /// <param name="message"></param>
        public void LogInformation(string message)
        {
        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(string message)
        {
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        public void LogError(string message, Exception? err = null)
        {
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="err"></param>
        public void LogError(Exception err)
        {
        }

        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        public void LogCritical(string message, Exception? err = null)
        {
        }

        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="err"></param>
        public void LogCritical(Exception err)
        {
        }


        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <typeparam name="TState"></typeparam>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // Do nothing
        }

        /// <summary>
        /// Returns true
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Does nothing and returns null.
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }
    }
}  