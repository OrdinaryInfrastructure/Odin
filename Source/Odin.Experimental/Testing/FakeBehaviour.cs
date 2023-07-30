namespace Odin.Testing
{
    /// <summary>
    /// Fake behaviour enum
    /// </summary>
    public enum FakeBehaviour
    {
        /// <summary>
        /// Succeed
        /// </summary>
        ReturnSuccessfulOutcome,
        
        /// <summary>
        /// Fail
        /// </summary>
        ReturnFailedOutcome,
        
        /// <summary>
        /// Return null
        /// </summary>
        ReturnNull,
        
        /// <summary>
        /// Throw an Exception
        /// </summary>
        ThrowException
    }
}

