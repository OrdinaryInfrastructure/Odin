using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Odin.DesignContracts;

namespace Odin.System
{
    /// <summary>
    /// Represents the outcome of an operation that can have any exception
    /// associated with a failure set in the Error property.
    /// Extends Outcome by addition of this Error property.
    /// </summary>
    public sealed class Outcome2 : Outcome
    {
        /// <summary>
        /// Any exception associated with a failure
        /// </summary>
        [JsonIgnore]
        public Exception? Error { get; }

        /// <summary>
        /// Whether an Error has been set.
        /// </summary>
        public bool HasError => Error!=null;

        /// <summary>
        /// Supported because of a requirement to be able to serialise Outcomes.
        /// Note: An uninitialised outcome is treated as a Failure FYI.
        /// </summary>
        public Outcome2() 
        {
        }
        
        /// <summary>
        /// Default constructor. Use Outcome.Succeed() for a successful Outcome with no message.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public Outcome2(bool success, string? message, Exception? error = null)
        {
            // PreCondition.Requires<ArgumentException>(!success || (success && error==null), "Error must be null for success");
            Success = success;
            if (!string.IsNullOrWhiteSpace(message))
            {
                _messages = new List<string> {message};
            }

            Error = error;
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="success"></param>
        /// <param name="messages"></param>
        /// <param name="error"></param>
        public Outcome2(bool success, IEnumerable<string> messages, Exception? error = null)
        {
            PreCondition.Requires<ArgumentException>(!success || (success && error!=null), "Error must be null for success");
            Success = success;
            _messages = messages.ToList();
            Error = error;
        }
        
        /// <summary>
        /// Success, including a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Outcome2 Succeed(string message)
        {
            return new Outcome2(true, message);
        }
        
        /// <summary>
        /// Success
        /// </summary>
        /// <returns></returns>
        public new static Outcome2 Succeed()
        {
            return new Outcome2(true, null as string);
        }
        
        /// <summary>
        /// Success
        /// </summary>
        /// <returns></returns>
        public static Outcome2 Fail(Exception err)
        {
            return new Outcome2(false,err.Message, err);
        }
        
        /// <summary>
        /// Failure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static Outcome2 Fail(string message)
        {
            return new Outcome2(false, message);
        }

    }
}