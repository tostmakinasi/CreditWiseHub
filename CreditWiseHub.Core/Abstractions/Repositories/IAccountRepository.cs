using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    /// <summary>
    /// Interface for managing account-related operations.
    /// </summary>
    public interface IAccountRepository : IGenericRepository<Account, Guid>
    {
        /// <summary>
        /// Gets an account based on its account number.
        /// </summary>
        /// <param name="accountNumber">The account number to search for.</param>
        /// <returns>The account associated with the provided account number, or null if not found.</returns>
        Task<Account> GetAccountByAccountNumber(string accountNumber);

        Task<Account> GetUserDefaultAccountByAccountNumberWithUserAndUserLimits(string userID);

        Task<Account> GetAccountByAccountNumberWithUserAndUserLimits(string accountNumber);

        /// <summary>
        /// Gets the full name of the account holder based on the account number.
        /// </summary>
        /// <param name="accountNumber">The account number to retrieve the account holder's full name for.</param>
        /// <returns>The full name of the account holder, or null if not found.</returns>
        Task<string> GetAccountHolderFullNameByAccountNumber(string accountNumber);

        /// <summary>
        /// Adds information about an external account to the repository.
        /// </summary>
        /// <param name="externalAccountInformation">The information about the external account to add.</param>
        Task AddExternalAccount(ExternalAccountInformation externalAccountInformation);

        /// <summary>
        /// Gets a list of accounts associated with a specific username.
        /// </summary>
        /// <param name="username">The username to retrieve accounts for.</param>
        /// <returns>A list of accounts associated with the provided username.</returns>
        Task<List<Account>> GetAccountsByUsername(string username);

        /// <summary>
        /// Gets an account with its transaction history based on the account number.
        /// </summary>
        /// <param name="accountNumber">The account number to retrieve the account information for.</param>
        /// <returns>The account with its transaction history, or null if not found.</returns>
        Task<Account> GetAccountsWithTransactionsByAccountNumber(string accountNumber);
    }

}
