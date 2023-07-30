using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

 namespace Odin.Logging
{
    /// <summary>
    /// Default ILoggerAdapter implementation wrapping ILogger of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;
        private readonly string _messagePrefix;
        
        /// <summary>
        /// Default constructor requires ILogger of T
        /// </summary>
        /// <param name="logger"></param>
        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
            try
            {
                Type typeParameterType = typeof(T);
                if (typeParameterType != null)
                {
                    _messagePrefix = typeParameterType.Name + ": ";
                }
                else
                {
                    _messagePrefix = "";
                }
            }
            catch (Exception err)
            {
                _logger.Log(LogLevel.Error, err, "LoggerAdapter: Unable to ascertain generic type");
                _messagePrefix = "";
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="err"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string? message, Exception? err = null)
        {
            _logger.Log(level, err, _messagePrefix + message);
        }
        
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string message)
        {
            Log(level, message, null);
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
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="argsToLogAsJson"></param>
        public void LogToJson(LogLevel level, params object[] argsToLogAsJson)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    MaxDepth = 4,
                    WriteIndented = true,
                    IncludeFields = false
                };
                string json = JsonSerializer.Serialize(argsToLogAsJson,options);
                Log(level, Environment.NewLine + json);
            }
            catch (Exception err)
            {
                Log(level, "LogToJson serialization error" + err.Message);
            }
        }

        /// <summary>
        /// LogTrace
        /// </summary>
        /// <param name="message"></param>
        public void LogTrace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        /// <summary>
        /// LogDebug
        /// </summary>
        /// <param name="message"></param>
        public void LogDebug(string message)
        {
            Log(LogLevel.Debug, message);
        }
        

        /// <summary>
        /// LogInformation
        /// </summary>
        /// <param name="message"></param>
        public void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        public void LogError(string message, Exception? err = null)
        {
            Log(LogLevel.Error, message, err);
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="err"></param>
        public void LogError(Exception err)
        {
            Log(LogLevel.Error, err);
        }
        
        
        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        public void LogCritical(string message, Exception? err = null)
        {
            Log(LogLevel.Critical, message, err);
        }

        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="err"></param>
        public void LogCritical(Exception err)
        {
            Log(LogLevel.Critical, null, err);
        }

        /// <summary>
        /// Writes a log entry.. Simply wraps the inner logger
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <typeparam name="TState"></typeparam>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        /// <summary>
        /// Checks if the given logLevel is enabled.. Simply wraps the inner logger
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        /// <summary>
        /// Begins a logical operation scope.. Simply wraps the inner logger
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _logger.BeginScope<TState>(state);
        }
    }
}