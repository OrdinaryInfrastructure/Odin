namespace Odin
{
    /// <summary>
    /// Represents the result of an operation that can succeed or fail,
    /// with a list of msessages of type TMessage.
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
        /// Needed to serialization.
        /// Success defaults to false.
        /// </summary>
        protected Result()
        {
            Success = false;
        }
        
        /// <summary>
        /// Result constructor.
        /// </summary>
        /// <param name="success">True or False</param>
        /// <param name="message">Optional message. Best practice is to include at least 1 message for failed operations however.</param>
        protected Result(bool success, TMessage? message = null)
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
        /// Default constructor. Use ResultValue.Succeed() for a successful Outcome with no message.
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