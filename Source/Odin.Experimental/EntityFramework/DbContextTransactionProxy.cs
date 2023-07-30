
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Odin.EntityFramework
{
    /// <summary>
    ///     This is a proxy to abstract SQL transaction beginning, committing and rolling back.
    /// </summary>
    public sealed class DbContextTransactionProxy : IDbContextTransactionProxy
    {
        private IDbContextTransaction _transaction;
        private readonly DbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public DbContextTransactionProxy(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Commit changes to the database
        /// </summary>
        public async Task BeginTransaction()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        
        /// <summary>
        /// Commit changes to the database
        /// </summary>
        public void Commit()
        {
            if (_transaction == null) throw new Exception("BeginTransaction() before calling Commit()");
            _transaction.Commit();
        }
        
        /// <summary>
        /// Rollback changes
        /// </summary>
        public void Rollback()
        {
            if (_transaction == null) throw new Exception("BeginTransaction() before calling Rollback()");
            _transaction.Rollback();
        }

        /// <summary>
        /// Dispose...
        /// </summary>
        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}