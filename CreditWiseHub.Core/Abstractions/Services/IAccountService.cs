using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// An interface that defines a service for managing account operations.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Creates a new account with the given username and account creation information and returns the account information.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="createAccountDto">The account creation information.</param>
        /// <returns>The account information.</returns>
        Task<Response<AccountInfoDto>> CreateByUserName(string username, CreateAccountDto createAccountDto);

        /// <summary>
        /// Checks the existence and recipient information of the account by the given account number.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The recipient account information.</returns>
        Task<Response<RecipientAccountInfoDto>> CheckAccountByAccountNumber(string accountNumber);

        /// <summary>
        /// Deposits money to the account by the given account number and amount information and returns the account's latest status.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="amount">The amount information.</param>
        /// <returns>The account's latest status.</returns>
        Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amount);

        /// <summary>
        /// Withdraws money from the account by the given account number and amount information and returns the account's latest status.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="amountDto">The amount information.</param>
        /// <returns>The account's latest status.</returns>
        Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto);

        /// <summary>
        /// Transfers money from the given account number to another account in the same bank by the transfer information and returns the transfer status.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="transferDto">The transfer information.</param>
        /// <returns>The transfer status.</returns>
        Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto);

        /// <summary>
        /// Transfers money from the given account number to another account in a different bank by the transfer information and returns the transfer status.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="transferDto">The transfer information.</param>
        /// <returns>The transfer status.</returns>
        Task<Response<TransactionStatusDto>> ExternalTransfer(string accountNumber, MoneyExternalTransferDto transferDto);

        /// <summary>
        /// Returns the account information by the given account number.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The account information.</returns>
        Task<Response<AccountInfoDto>> GetAccountInfoByAccountNumber(string accountNumber);

        /// <summary>
        /// Returns the information of the accounts that the user owns by the given username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user's accounts information.</returns>
        Task<Response<UserAccountsInfoDto>> GetUserAccountsByUsername(string username);

        /// <summary>
        /// Returns whether the account belongs to the user by the given account number and user id.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>True if the account belongs to the user, false otherwise.</returns>
        Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId);

        /// <summary>
        /// Updates the account by the given account number and update information and returns the result.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="updateAccountDto">The update information.</param>
        /// <returns>The result.</returns>
        Task<Response<NoDataDto>> UpdateAccountAsync(string accountNumber, UpdateAccountDto updateAccountDto);

        /// <summary>
        /// Returns the account's past transactions by the given account number.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The account's past transactions.</returns>
        Task<Response<AccountHistoryDto>> GetAccountHistoryByAccountNumber(string accountNumber);

        /// <summary>
        /// Transfers credit to the user's account by the given user id and credit amount and returns the transfer status.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="creditAmount">The credit amount.</param>
        /// <returns>The transfer status.</returns>
        Task<Response<TransactionStatusDto>> TransferCreditToAccountByUserId(string userId, decimal creditAmount);

        /// <summary>
        /// Withdraws money from the account for automatic payment by the given payment information and returns the payment result.
        /// </summary>
        /// <param name="dto">The payment information.</param>
        /// <returns>The payment result.</returns>
        Task<Response<PaymentProcessResultDto>> WithdrawMoneyForAutomaticPayment(PaymentProcessDto dto);

        /// <summary>
        /// Closes the account by the given account number and returns the result.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The result.</returns>
        Task<Response<NoDataDto>> CloseAccount(string accountNumber);

        /// <summary>
        /// Returns the account's balance information by the given account number.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The account's balance information.</returns>
        Task<Response<AccountBalanceInfoDto>> GetAccountBalanceByAccountNumber(string accountNumber);
    }

}
