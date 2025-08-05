using Odin.DesignContracts;

namespace Odin.System
{
    /// <summary>
    /// Represents the outcome of an operation that can succeed or fail, with a simple list of string Messages.
    /// </summary>
    /// <remarks>To be renamed to Result</remarks>
    public record Outcome : Result<string>
    {
        /// <inheritdoc />
        public Outcome()
        {
        }

        /// <summary>
        /// Default constructor. Use Outcome.Succeed() for a successful Outcome with no message.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public Outcome(bool success, string? message = null) : base(success, message)
        {
        }

        /// <inheritdoc />
        public Outcome(bool success, IEnumerable<string>? messages = null) : base(success, messages)
        {
        }

        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Outcome Fail(string? message)
        {
            return new Outcome(false, message);
        }

        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public new static Outcome Fail(IEnumerable<string> messages)
        {
            return new Outcome(false, messages);
        }

        /// <summary>
        /// Success
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Outcome Succeed(string? message = null)
        {
            return new Outcome(true, message);
        }

        /// <summary>
        /// Success
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public new static Outcome Succeed(IEnumerable<string> messages)
        {
            return new Outcome(true, messages);
        }

        /// <summary>
        /// All messages flattened into 1 message.
        /// </summary>
        public string MessagesToString(string separator = " | ")
        {
            if (_messages == null || _messages.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(separator, Messages);
        }

        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Outcome<TValue> Fail<TValue>(string? message)
        {
            return new Outcome<TValue>(false, default(TValue), message);
        }

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Outcome<TValue> Succeed<TValue>(TValue value, string? message = null)
        {
            return new Outcome<TValue>(true, value, message);
        }
    }

    /// <summary>
    /// Represents the success or failure of an operation that returns a Value\Result on success,
    /// and list of string messages.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>To be renamed to ResultValue of TValue</remarks>
    public record Outcome<TValue> : ResultValue<TValue, string>
    {
        /// <summary>
        /// Parameterless constructor for serialization.
        /// Note that Success == false is the default for an uninitialised Outcome.
        /// </summary>
        public Outcome()
        {
            Success = false;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="messages">Optional, but good practice is to provide messages for failed results.</param>
        public Outcome(bool success, TValue? value, IEnumerable<string>? messages)
        {
            PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
            Value = value;
            _messages = messages?.ToList();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="message">Optional, but good practice is to provide messages for failed results.</param>
        public Outcome(bool success, TValue? value, string? message = null)
        {
            PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
            Success = success;
            Value = value;
            _messages = message != null ? [message] : null;
        }

        /// <summary>
        /// All messages flattened into 1 message.
        /// </summary>
        public string MessagesToString(string separator = " | ")
        {
            if (_messages == null || _messages.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(separator, Messages);
        }
    }
}