using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account, Guid>
    {
        Task<Account> GetAccountByAccountNumber(string accountNumber);

        Task<string> GetAccountHolderFullNameByAccountNumber(string accountNumber);

        Task AddExternalAccount(ExternalAccountInformation externalAccountInformation);

        Task<List<Account>> GetAccountsByUsername(string username);

        Task<Account> GetAccountsWithTransactionsByAccountNumber(string accountNumber);
    }
}
