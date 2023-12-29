namespace CreditWiseHub.Core.Abstractions.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task CommitAsync();

        void Commit();

        Task BeginTransactionAsync();

        Task TransactionCommitAsync();
    }
}
