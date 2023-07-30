using System.Collections.Generic;
using System.Linq;

namespace Odin.System
{
    /// <summary>
    /// Represents the outcome of an operation
    /// </summary>
    public class Outcome
    {
        private const string DefaultFailureMessage = "An uninitialised outcome is a failure by default.";
        
        /// <summary>
        /// Messages list
        /// </summary>
        protected List<string>? _messages;

        /// <summary>
        /// An uninitialised outcome is deemed a Failure BTW>
        /// </summary>
        public Outcome()
        {
            Success = false;
        }

        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }

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
        /// Messages
        /// </summary>
        public List<string> Messages
        {
            get
            {
                // Return default message if a failure and there are no messages.
                if (!Success && (_messages == null || _messages.Count == 0))
                    return new List<string> { DefaultFailureMessage };
                if (_messages == null)
                {
                    _messages = new List<string>();
                }
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }

        /// <summary>
        /// Default constructor. Use Outcome.Succeed() for a successful Outcome with no message.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public Outcome(bool success, string? message)
        {
            Success = success;
            if (!string.IsNullOrWhiteSpace(message))
            {
                _messages = new List<string> {message};
            }
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="success"></param>
        /// <param name="messages"></param>
        public Outcome(bool success, IEnumerable<string> messages)
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
        public static Outcome Fail(string message)
        {
            return new Outcome(false, message);
        }

        /// <summary>
        /// Creates a Failure outcome with messages.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static Outcome Fail(IEnumerable<string> messages)
        {
            return new Outcome(false, messages);
        }

        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Outcome<T> Fail<T>(string message)
        {
            return new Outcome<T>(default(T), false, message);
        }
        
        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="messages"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Outcome<T> Fail<T>(IEnumerable<string> messages)
        {
            return new Outcome<T>(default(T), false, messages);
        }
        
        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Outcome<T> Fail<T>(T value, string message)
        {
            return new Outcome<T>(value, false, message);
        }


        /// <summary>
        /// Success
        /// </summary>
        /// <returns></returns>
        public static Outcome Succeed()
        {
            return new Outcome(true, null as string);
        }
        
        /// <summary>
        /// Success, including a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Outcome Succeed(string message)
        {
            return new Outcome(true, message);
        }
        
        /// <summary>
        /// Success, including messages
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static Outcome Succeed(IEnumerable<string> messages)
        {
            return new Outcome(true, messages);
        }

        /// <summary>
        /// Success
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Outcome<T> Succeed<T>(T value)
        {
            return new Outcome<T>(value, true, null as string);
        }
        
        /// <summary>
        /// Success
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Outcome<T> Succeed<T>(T value, string? message)
        {
            return new Outcome<T>(value, true, message);
        }
        
        /// <summary>
        /// Success
        /// </summary>
        /// <param name="value"></param>
        /// <param name="messages"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Outcome<T> Succeed<T>(T value, IEnumerable<string> messages)
        {
            return new Outcome<T>(value, true, messages);
        }

        /// <summary>
        /// Returns Success only if all succeed.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static Outcome Combine(params Outcome[] results)
        {
            foreach (Outcome result in results)
            {
                if (!result.Success)
                    return result;
            }

            return Succeed();
        }
    }

    /// <summary>
    /// Represents the success or failure of an operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Outcome<T> : Outcome
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Outcome() 
        {
            
        }
        
        private T? _value;

        /// <summary>
        /// Important: Value is not null when Success is True. Value can be null when Success is false.
        /// </summary>
        public T Value
        {
            get
            {
                return _value!;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="success"></param>
        /// <param name="message"></param>
        protected internal Outcome(T? value, bool success, string? message)
            : base(success, message)
        {
            _value = value;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="success"></param>
        /// <param name="messages"></param>
        protected internal Outcome(T? value, bool success, IEnumerable<string> messages)
            : base(success, messages)
        {
            _value = value;
        }
        
    }
}