﻿using Microsoft.Extensions.Logging;

namespace Odin.Logging
{
    /// <summary>
    /// Logging adapter interface to wrap ILogger mainly to ease mocking in tests...
    /// </summary>
    /// <typeparam name="TCategoryName"></typeparam>
    public interface IMockableLogger<out TCategoryName> : IMockableLogger
    {

    }

    /// <summary>
    /// Logging adapter interface to wrap ILogger mainly to ease mocking in tests
    /// </summary>
    public interface IMockableLogger : ILogger
    {
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="error"></param>
        void Log(LogLevel level, string? message = null, Exception? error = null);

        /// <summary>
        /// Structured Log Message. args object array contains a list of values to populate
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
            params object?[] args);
        
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="err"></param>
        void Log(LogLevel level, Exception err);

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="argsToLogAsJson"></param>
        void LogToJson(LogLevel level, params object[] argsToLogAsJson);
        
        /// <summary>
        /// LogTrace
        /// </summary>
        /// <param name="message"></param>
        void LogTrace(string message);
        
        /// <summary>
        /// LogDebug
        /// </summary>
        /// <param name="message"></param>
        void LogDebug(string message);
        
        /// <summary>
        /// LogInformation
        /// </summary>
        /// <param name="message"></param>
        void LogInformation(string message);
        
        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="message"></param>
        void LogWarning(string message);
        
        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        void LogError(string message, Exception? err = null);
        
        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="err"></param>
        void LogError(Exception err);
        
        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        void LogCritical(string message, Exception? err = null);
        
        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="err"></param>
        void LogCritical(Exception err);


    }
}  