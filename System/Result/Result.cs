namespace Odin.System
{
    /// <summary>
    /// Represents the outcome of an operation that can succeed or fail, with a list of string Messages.
    /// </summary>
    /// <remarks>Previously named Outcome</remarks>
    /// <remarks>To be renamed to Result</remarks>
    public record Result : Result<string>
    {
        /// <inheritdoc />
        public Result() 
        {
        }
        
        /// <summary>
        /// Default constructor. Use Outcome.Succeed() for a successful Outcome with no message.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public Result(bool success, string? message = null) : base(success, message)
        {
        }

        /// <inheritdoc />
        public Result(bool success, IEnumerable<string>? messages = null) : base(success, messages)
        {
        }

        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Result Fail(string? message)
        {
            return new Result(false, message);
        }
        
        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public new static Result Fail(IEnumerable<string> messages)
        {
            return new Result(false, messages);
        }

        /// <summary>
        /// Success
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Result Succeed(string? message = null)
        {
            return new Result(true, message);
        }
        
        /// <summary>
        /// Success
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public new static Result Succeed(IEnumerable<string> messages)
        {
            return new Result(true, messages);
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

    /// <summary>
    /// Represents the result of an operation that can succeed or fail, with a list of messages\errors of type TMessage.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public record Result<TMessage> where TMessage : class
    {
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Messages list
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected List<TMessage>? _messages;

        /// <summary>
        /// Messages
        /// </summary>
        public IReadOnlyList<TMessage> Messages
        {
            get
            {
                return _messages ??= new List<TMessage>();       
            }
            init  // For deserialisation
            {
                _messages = value.ToList();
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
            if (message != null)
            {
                _messages = new List<TMessage> { message };
            }
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
        /// Success, including a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result<TMessage> Succeed(TMessage? message = null)
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
            Assertions.RequiresArgumentPrecondition(!(value == null && success), "Value is required for a successful result.");
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
            Assertions.RequiresArgumentPrecondition(!(value == null && success), "Value is required for a successful result.");
            Success = success;
            Value = value;
            _messages = message != null ? [message] : null;
        }
        
        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static ResultValue<TValue> Fail(string? message)
        {
            return new ResultValue<TValue>(false, default(TValue), message);
        }
        
        /// <summary>
        /// Failure.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultValue<TValue?> Fail(TValue? value, string? message = null)
        {
            return new ResultValue<TValue?>(false, value, message);
        }
        
        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static ResultValue<TValue> Succeed(TValue value, string? message = null)
        {
            return new ResultValue<TValue>(true, value, message);
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
        public TValue? Value { get; init; }

        // /// <summary>
        // /// Underlying value.
        // /// </summary>
        // private TValue? _value;

        /// <summary>
        /// Parameterless constructor for serialisation, etc.
        /// </summary>
        public ResultValue()
        {
            Value = default(TValue);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="success">true or false</param>
        /// <param name="value">Required if successful</param>
        /// <param name="messages">Optional, but good practice is to provide messages for failed results.</param>
        protected ResultValue(bool success, TValue? value, IEnumerable<TMessage>? messages)
        {
            Assertions.RequiresArgumentPrecondition(!(value == null && success), "Value is required for a successful result.");
            Value = value;
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
            Assertions.RequiresArgumentPrecondition(!(value == null && success), "Value is required for a successful result.");
            Value = value;
            _messages = message != null ? [message] : null;
        }

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="value">Required.</param>
        /// <param name="messages">Not normally used for successful operations, but can be for informational purposes.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ResultValue<TValue, TMessage> Succeed(TValue value, IEnumerable<TMessage>? messages)
        {
            Assertions.RequiresArgumentNotNull(value);
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
            Assertions.RequiresArgumentNotNull(value);
            return new ResultValue<TValue, TMessage>(true, value, new List<TMessage>() { message });
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
            return new ResultValue<TValue, TMessage>(false, default(TValue), new List<TMessage>() { message });
        }
    }

    internal static class Assertions
    {
        internal static void RequiresArgumentPrecondition(bool requirement, string argumentRequirementMessage)
        {
            if (!requirement) throw new ArgumentException(argumentRequirementMessage);
        }

        internal static void RequiresArgumentNotNull(object? argument, string? argumentIsRequiredMessage = null)
        {
            if (argument != null) return;
            if (string.IsNullOrWhiteSpace(argumentIsRequiredMessage))
            {
                argumentIsRequiredMessage = $"{nameof(argument)} is required";
            }
            ArgumentNullException ex = new ArgumentNullException(argumentIsRequiredMessage);
            throw ex;
        }
    }
}