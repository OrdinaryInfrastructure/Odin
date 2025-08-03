using Odin.DesignContracts;

namespace Odin.System
{
    /// <inheritdoc />
    /// <remarks>Supporting refactoring of Outcome -> Result</remarks>
    [Obsolete("Use Result. Will be deprecated.")]
    public record Outcome : Result 
    {
    }

    /// <inheritdoc />
    /// <remarks>Supporting refactoring of Outcome -> ResultValue</remarks>
    [Obsolete("Use ResultValue<TValue>. Will be deprecated.")]
    public record Outcome<TValue> : ResultValue<TValue>
    {
        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Outcome<TValue> Succeed<TValue>(TValue value, string? message = null)
        {
            return new ResultValue<TValue>(true, value, message);
        }
    }
    

    /// <summary>
    /// Represents the success or failure of an operation that returns a Value\Result on success,
    /// and list of string messages.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>To be renamed to ResultValue of TValue</remarks>
    public record ResultValue<TValue> : ResultValue<TValue, string>
    {
        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        public ResultValue()
        {
            Success = false;
        }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="messages">Optional, but good practice is to provide messages for failed results.</param>
        public ResultValue(bool success, TValue? value, IEnumerable<string>? messages)
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
        public ResultValue(bool success, TValue? value, string? message = null)
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