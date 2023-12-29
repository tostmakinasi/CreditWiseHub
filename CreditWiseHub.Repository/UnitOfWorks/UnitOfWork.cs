using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Repository.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace CreditWiseHub.Repository.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task TransactionCommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _transaction.Commit();
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
            }
        }
    }
}
