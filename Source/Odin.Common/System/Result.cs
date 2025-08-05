namespace Odin.System;

    /// <summary>
    /// Represents the result of an operation that can succeed or fail,
    /// with a generic list of messages\errors of type TMessage.
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
