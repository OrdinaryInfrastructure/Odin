namespace Odin.DesignContracts;

public static partial class Contract
{
           

        /// <summary>
        /// Specifies a postcondition that must hold true when the enclosing method returns.
        /// </summary>
        /// <param name="condition">The condition that must be <c>true</c>.</param>
        /// <param name="userMessage">An optional message describing the postcondition.</param>
        /// <param name="conditionText">An optional text representation of the condition expression.</param>
        /// <remarks>
        /// Postconditions are evaluated only when <see cref="ContractRuntime.PostconditionsEnabled"/> is <c>true</c>.
        /// Calls to this method become no-ops when postconditions are disabled.
        /// It is expected that source-generated code will invoke this method at
        /// appropriate points (typically immediately before method exit).
        /// </remarks>
        public static void Ensures(bool condition, string? userMessage = null, string? conditionText = null)
        {
            if (!ContractRuntime.PostconditionsEnabled)
            {
                return;
            }

            if (!condition)
            {
                ReportFailure(
                    ContractFailureKind.Postcondition,
                    userMessage,
                    conditionText);
            }
        }

        
        /// <summary>
        /// Specifies an object invariant that must hold true whenever the object is in a valid state.
        /// </summary>
        /// <param name="condition">The condition that must be <c>true</c>.</param>
        /// <param name="userMessage">An optional message describing the invariant.</param>
        /// <param name="conditionText">An optional text representation of the condition expression.</param>
        /// <remarks>
        /// Invariants are evaluated only when <see cref="ContractRuntime.InvariantsEnabled"/> is <c>true</c>.
        /// Calls to this method become no-ops when invariants are disabled.
        /// It is expected that source-generated code will invoke invariant methods
        /// (marked with <see cref="ContractInvariantMethodAttribute"/>) at appropriate points.
        /// </remarks>
        public static void Invariant(bool condition, string? userMessage = null, string? conditionText = null)
        {
            if (!ContractRuntime.InvariantsEnabled)
            {
                return;
            }

            if (!condition)
            {
                ReportFailure(
                    ContractFailureKind.Invariant,
                    userMessage,
                    conditionText);
            }
        }


        /// <summary>
        /// Specifies an assertion that must hold true at the given point in the code.
        /// </summary>
        /// <param name="condition">The condition that must be <c>true</c>.</param>
        /// <param name="userMessage">An optional message describing the assertion.</param>
        /// <param name="conditionText">An optional text representation of the condition expression.</param>
        /// <remarks>
        /// Assertions are always evaluated at runtime.
        /// </remarks>
        public static void Assert(bool condition, string? userMessage = null, string? conditionText = null)
        {
            if (!condition)
            {
                ReportFailure(
                    ContractFailureKind.Assert,
                    userMessage,
                    conditionText);
            }
        }

        /// <summary>
        /// Specifies an assumption that the analysis environment may rely on.
        /// </summary>
        /// <param name="condition">The condition that is assumed to be <c>true</c>.</param>
        /// <param name="userMessage">An optional message describing the assumption.</param>
        /// <param name="conditionText">An optional text representation of the condition expression.</param>
        /// <remarks>
        /// At runtime, <see cref="Assume"/> behaves identically to <see cref="Assert"/>,
        /// but analyzers may interpret assumptions differently.
        /// </remarks>
        public static void Assume(bool condition, string? userMessage = null, string? conditionText = null)
        {
            if (!condition)
            {
                ReportFailure(
                    ContractFailureKind.Assume,
                    userMessage,
                    conditionText);
            }
        }
 
}