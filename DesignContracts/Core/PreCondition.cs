#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Odin.DesignContracts
{
    /// <summary>
    /// Represents a requirement needing to be met by a calling client.
    /// </summary>
    public static class PreCondition
    {
        private const string ObsoleteMessage = "Moved to Contract.Requires";
        
        /// <summary>
        /// Evaluates the contract condition required, else throws an Exception.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="failureMessage"></param>
        [Obsolete(ObsoleteMessage)]
        public static void Requires(bool condition, string? failureMessage = null)
        {
            Contract.Requires(condition, failureMessage);
        }

        [Obsolete(ObsoleteMessage)]
        public static void Requires<TException>(bool condition, string? failureMessage = null)
            where TException : Exception
        {
            Contract.Requires<TException>(condition, failureMessage);
        }
    }
}