namespace Odin.DesignContracts
{
    /// <summary>
    /// Represents a contract needing to be met by the caller of a public sealed class method on entry to said method.
    /// </summary>
    public static class PreCondition
    {
        /// <summary>
        /// 'Precondition failure'
        /// </summary>
        public const string FailureText = "Precondition failure";
        
        /// <summary>
        /// Evaluates the contract condition required, else throws an Exception.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="failureMessage"></param>
        public static void Requires(bool condition, string? failureMessage = null)
        {
            if (condition) return;
            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                failureMessage = FailureText;
            }

            throw new Exception(failureMessage);
        }

        /// <summary>
        /// Evaluates the contract condition required, else throws a TException.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="failureMessage"></param>
        /// <typeparam name="TException"></typeparam>
        public static void Requires<TException>(bool condition, string? failureMessage = null)
            where TException : Exception
        {
            if (condition) return;
            throw new ExceptionBuilder<TException>(failureMessage, FailureText).Build();
        }

        /// <summary>
        /// Commonly used 'argument not null' precondition check.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="failureMessage"></param>
        public static void RequiresNotNull(object? argument, string? failureMessage = null)
        {
            if (argument != null) return;
            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                failureMessage = $"{nameof(argument)} is required";
            }

            ArgumentNullException ex = new ArgumentNullException(failureMessage);
            throw ex;
        }

        /// <summary>
        /// Commonly used 'argument not null or empty' precondition check.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="failureMessage"></param>
        public static void RequiresNotNullOrWhitespace(string? argument, string? failureMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(argument)) return;
            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                failureMessage = $"{nameof(argument)} must not be null, empty or whitespace.";
            }

            ArgumentException ex = new ArgumentException(failureMessage);
            throw ex;
        }
    }
}