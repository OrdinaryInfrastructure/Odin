using System;
using Microsoft.Extensions.Logging;
using Moq;
using Odin.Logging;

namespace Tests.Odin.Logging
{
    public sealed class LoggerAdapterTestBuilder<T>
    {
        public LoggerAdapterTestBuilder()
        {
            LoggerMock = new Mock<ILogger<T>>();
        }

        public Mock<ILogger<T>> LoggerMock { get; }

        public LoggerAdapter<T> CreateLogger()
        {
            return new LoggerAdapter<T>(LoggerMock.Object);
        }

        /// <summary>
        /// Verifies that a Log call has been made, with the given LogLevel, Message and optional KeyValuePairs.
        /// </summary>
        /// <param name="expectedLogLevel">The LogLevel to verify.</param>
        /// <param name="expectedMessage">The Message to verify.</param>
        /// <param name="expectedException">The Message to verify.</param>
        /// <param name="times">The number of invocations.</param>
        public void VerifyLog(LogLevel expectedLogLevel,
            string expectedMessage, Times times, Exception expectedException = null)
        {
            // Todo: Fix this...
            // Looks like the ridiculous ILogger.Log method below in ILogger will be expanded in .Net 5
            if (expectedException == null)
            {
                LoggerMock.Verify(mock => mock.Log(
                        expectedLogLevel,
                        It.IsAny<EventId>(),
                        It.IsAny<object>(),
                        (Exception)null,
                        It.IsAny<Func<object, Exception, string>>()
                    ), times
                );
            }

            LoggerMock.Verify(mock => mock.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    expectedException,
                    It.IsAny<Func<object, Exception, string>>()
                ), times
            );
        }
    }
}