using Odin.DesignContracts;

namespace Odin.System
{
    /// <summary>
    /// Represents the outcome of an operation that can succeed or fail, with a simple list of string Messages.
    /// </summary>
    /// <remarks>To be renamed to Result</remarks>
    public record Outcome : Result<string>
    {
        /// <summary>
        /// Default constructor. Use Outcome.Succeed() for a successful Outcome with no message.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public Outcome(bool success, string? message = null) : base(success, message)
        {
        }
        
        private const string DefaultFailureMessage = "An uninitialised outcome is a failure by default.";

        /// <summary>
        /// All messages flattened into 1 message.
        /// </summary>
        public string MessagesToString(string separator = " | ")
        {
            if (_messages == null || _messages.Count == 0)
            {
                if (Success)
                    return string.Empty;
                return DefaultFailureMessage;
            }

            return string.Join(separator, Messages);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Outcome<TValue> Fail<TValue>(string? message)
        {
            return new Outcome<TValue>(false, default(TValue), message);
        }
    }


    /// <summary>
    /// Represents the result of an operation that can succeed or fail, with a list of messages\errors of type TMessage.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public record Result<TMessage> where TMessage : class
    {
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; protected set; }

        /// <summary>
        /// Messages list
        /// </summary>
        protected List<TMessage>? _messages;

        /// <summary>
        /// Messages
        /// </summary>
        public IReadOnlyList<TMessage> Messages
        {
            get
            {
                // Return default message if a failure and there are no messages.
                // if (!Success && (_messages == null || _messages.Count == 0))
                //     return new List<TMessage> { DefaultFailureMessage };
                if (_messages == null)
                {
                    _messages = new List<TMessage>();
                }
                return _messages;
            }
        }

        /// <summary>
        /// Result constructor. Default Success value is false. 
        /// </summary>
        /// <param name="success">True or False</param>
        /// <param name="message">Optional message. Best practice is to include at least 1 message for failed operations however.</param>
        protected Result(bool success = false, TMessage? message = null)
        {
            Success = success;
            _messages = new List<TMessage> { message };
        }


        /// <summary>
        /// Result constructor.
        /// </summary>
        /// <param name="success">True or False</param>
        /// <param name="messages">Optional message. Best practice is to include at least 1 message for failed operations however.</param>
        public Result(bool success, IEnumerable<TMessage>? messages)
        {
            Success = success;
            if (messages != null)
            {
                _messages = messages.ToList();
            }
        }

        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result<TMessage> Fail(TMessage message)
        {
            return new Result<TMessage>(false, message);
        }

        /// <summary>
        /// Creates a Failure outcome with messages.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns> 
        public static Result<TMessage> Fail(IEnumerable<TMessage> messages)
        {
            return new Result<TMessage>(false, messages);
        }

        /// <summary>
        /// Success
        /// </summary>
        /// <returns></returns>
        public static Result<TMessage> Succeed()
        {
            return new Result<TMessage>(true);
        }

        /// <summary>
        /// Success, including a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result<TMessage> Succeed(TMessage message)
        {
            return new Result<TMessage>(true, message);
        }

        /// <summary>
        /// Success, including messages
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static Result<TMessage> Succeed(IEnumerable<TMessage> messages)
        {
            return new Result<TMessage>(true, messages);
        }

        /// <summary>
        /// Returns Success only if all succeed, else returns the first failure.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static Result<TMessage> Combine(params Result<TMessage>[] results)
        {
            foreach (Result<TMessage> result in results)
            {
                if (!result.Success)
                    return result;
            }

            return Succeed();
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
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="messages">Optional, but good practice is to provide messages for failed results.</param>
        private Outcome(bool success, TValue? value, IEnumerable<string>? messages = null)
        {
            PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
            _value = value;
            _messages = messages?.ToList();
        }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="message">Optional, but good practice is to provide messages for failed results.</param>
        private Outcome(bool success, TValue? value, string? message = null)
        {
            PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
            _value = value;
            _messages = message != null ? [message] : null;
        }
        

    }

    /// <summary>
    /// Represents the success or failure of an operation that returns a Value\Result on success,
    /// and list of messages, of type TMessage.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public record ResultValue<TValue, TMessage> : Result<TMessage> where TMessage : class
    {
        /// <summary>
        /// Value is not null when Success is True. Value is null when Success is false.
        /// </summary>
        public TValue? Value => _value;

        /// <summary>
        /// Underlying value.
        /// </summary>
        protected TValue? _value;

        /// <summary>
        /// Parameterless constructor for serialisation, etc.
        /// </summary>
        public ResultValue()
        {
            _value = default(TValue);
            _messages = null;
        }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="messages">Optional, but good practice is to provide messages for failed results.</param>
        protected ResultValue(bool success, TValue? value, IEnumerable<TMessage>? messages)
        {
            PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
            _value = value;
            _messages = messages?.ToList();
        }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="message">Optional, but good practice is to provide messages for failed results.</param>
        protected ResultValue(bool success, TValue? value, TMessage? message = null)
        {
            PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
            _value = value;
            _messages = message != null ? [message] : null;
        }

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="value">Required.</param>
        /// <param name="messages">Not normally used for successful operations, but can be for informational purposes.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ResultValue<TValue, TMessage> Succeed(TValue value, IEnumerable<TMessage>? messages = null)
        {
            PreCondition.RequiresNotNull(value);
            return new ResultValue<TValue, TMessage>(true, value, messages);
        }

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="value">Required.</param>
        /// <param name="message">Not normally used for successful operations, but can be for informational purposes.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ResultValue<TValue, TMessage> Succeed(TValue value, TMessage? message = null)
        {
            PreCondition.RequiresNotNull(value);
            return new ResultValue<TValue, TMessage>(true, value, new List<TMessage>(){message});
        }

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="messages">Normally included as best practice for failed operations, but not mandatory.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ResultValue<TValue, TMessage> Fail(IEnumerable<TMessage>? messages = null)
        {

            return new ResultValue<TValue, TMessage>(false, default(TValue), messages);
        }

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="message">Normally included as best practice for failed operations, but not mandatory.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ResultValue<TValue, TMessage> Fail(TMessage? message = null)
        {
            return new ResultValue<TValue, TMessage>(false, default(TValue), new List<TMessage>(){message});
        }
    }
}