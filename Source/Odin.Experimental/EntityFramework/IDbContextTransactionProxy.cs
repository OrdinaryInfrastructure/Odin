namespace Odin.EntityFramework
{
    /// <summary>
    /// Abstraction of BeginTransaction, Commit and Rollback to assist with testability.
    /// </summary>
    public interface IDbContextTransactionProxy : IDisposable
    {
        /// <summary>
        /// Begins the transaction...
        /// </summary>
        /// <returns></returns>
        Task BeginTransaction();
        
        /// <summary>
        /// Commit
        /// </summary>
        void Commit();

        /// <summary>
        ///  Rollback
        /// </summary>
        void Rollback();
    }
}