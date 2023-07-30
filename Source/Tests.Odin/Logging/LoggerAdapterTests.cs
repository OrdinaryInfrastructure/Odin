using System;
using Odin.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.Odin.Logging
{
    [TestFixture]
    public sealed class LoggerAdapterTests
    {
        [Test][Ignore("Verifying ILogger.Log needs to be worked out...")]
        public void LogCritical_logs_critical_with_message()
        {
            LoggerAdapterTestBuilder<string> loggerTestBuilder = new LoggerAdapterTestBuilder<string>();
            LoggerAdapter<string> underTest = loggerTestBuilder.CreateLogger();

            underTest.LogCritical("message");

            loggerTestBuilder.VerifyLog(LogLevel.Critical,"message", Times.Once());
        }

        [Test][Ignore("Verifying ILogger.Log needs to be worked out...")]
        public void LogCritical_logs_critical_with_exception()
        {
            LoggerAdapterTestBuilder<string> loggerTestBuilder = new LoggerAdapterTestBuilder<string>();
            LoggerAdapter<string> underTest = loggerTestBuilder.CreateLogger();
            Exception ex = new Exception("strange_love");

            underTest.LogCritical("message", ex);

            loggerTestBuilder.VerifyLog(LogLevel.Critical, "message", Times.Once(), ex);
        }
    }
}