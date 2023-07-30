using System;

 namespace Odin.DesignContracts
{
    /// <summary>
    /// Represents a contract against a method return value.
    /// </summary>
    public static class PostCondition
    {
        /// <summary>
        /// 'Precondition failure'
        /// </summary>
        public const string FailureText = "Postcondition failure";
        
        /// <summary>
        /// Ensures condition is met else throws a System.Exception
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="failureMessage"></param>
        public static void Ensures(bool condition, string? failureMessage  = null)
        {
            if (condition) return;
            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                failureMessage = FailureText;
            }
            throw new Exception(failureMessage);
        }

        /// <summary>
        /// Ensure condition is met else throws TException
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="failureMessage"></param>
        /// <typeparam name="TException"></typeparam>
        public static void Ensures<TException>(bool condition, string failureMessage ) where TException : Exception
        {
            if (condition) return;
            throw new ExceptionBuilder<TException>(failureMessage, FailureText).Build();
        }

        /// <summary>
        /// Inline implementation of post postConditions for class method return
        /// </summary>
        /// <param name="toBeReturned"></param>
        /// <param name="postConditionsDelegate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReturnAfterPostConditions<T>(this T toBeReturned, Action<T> postConditionsDelegate) 
        {
            postConditionsDelegate.Invoke(toBeReturned);
            return toBeReturned;
        }
        
    }
}
