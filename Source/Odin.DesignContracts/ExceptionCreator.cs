using Odin.System;

namespace Odin.DesignContracts
{
    internal sealed class ExceptionBuilder<TException> : IBuilder<Exception> where TException: Exception
    {
        private string? _exceptionMessage;
        private string _fallbackMessage;
        // private TException type;
        
        internal ExceptionBuilder(string? exceptionMessage, string fallbackMessage)
        {
            _exceptionMessage = exceptionMessage;
            _fallbackMessage = fallbackMessage;
        }

        public Exception Build()
        {
            string message = _exceptionMessage!;
            if (string.IsNullOrWhiteSpace(message))
            {
                message = _fallbackMessage;
            }

            TException? ex;
            try
            {
                ex = Activator.CreateInstance(typeof(TException), message) as TException;
            }
            catch
            {
                return new Exception(message);
            }

            if (ex != null) return ex;
            return new Exception(message);
        }
    }
}