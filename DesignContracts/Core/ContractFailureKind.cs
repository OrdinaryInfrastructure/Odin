namespace Odin.DesignContracts
{
    /// <summary>
    /// Defines the semantic category of a contract failure.
    /// </summary>
    public enum ContractFailureKind
    {
        /// <summary>
        /// The failure occurred because a precondition was not satisfied.
        /// </summary>
        Precondition = 0,

        /// <summary>
        /// The failure occurred because a postcondition was not satisfied.
        /// </summary>
        Postcondition = 1,

        /// <summary>
        /// The failure occurred because an object invariant was not satisfied.
        /// </summary>
        Invariant = 2,

        /// <summary>
        /// The failure occurred because an assertion did not hold.
        /// </summary>
        Assert = 3,

        /// <summary>
        /// The failure occurred because an assumption did not hold.
        /// </summary>
        Assume = 4
    }
}