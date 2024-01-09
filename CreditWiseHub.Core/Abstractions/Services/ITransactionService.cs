using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// An interface that defines a service for managing money transfers and deposits/withdrawals between accounts.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Transfers money between the given receiver and sender accounts and transfer information in the same bank and returns the transfer status.
        /// </summary>
        /// <param name="receiverAccount">The receiver account.</param>
        /// <param name="senderAccount">The sender account.</param>
        /// <param name="transferDto">The transfer information.</param>
        /// <returns>The transfer status.</returns>
        Task<Response<TransactionStatusDto>> CreateInternalTransaction(Account receiverAccount, Account senderAccount, MoneyTransferDto transferDto);

        /// <summary>
        /// Transfers money from the given account and transfer information to another account in a different bank and returns the transfer status.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="transferDto">The transfer information.</param>
        /// <returns>The transfer status.</returns>
        Task<Response<TransactionStatusDto>> CreateExternalTransaction(Account account, MoneyExternalTransferDto transferDto);

        /// <summary>
        /// Returns the status of the transaction by the given transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>The transaction status.</returns>
        Task<Response<TransactionStatusDto>> CheckTransactionStatus(long transactionId);

        /// <summary>
        /// Performs a deposit or withdrawal operation on the given account, amount, description, new opening, transaction type and pre approval information and returns the transaction status.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="description">The description.</param>
        /// <param name="isNewOpening">Indicates whether the account is newly opened.</param>
        /// <param name="transactionType">The transaction type.</param>
        /// <param name="preApproval">Indicates whether the operation is pre approved.</param>
        /// <returns>The transaction status.</returns>
        Task<Response<TransactionStatusDto>> AddDepositWithdrawalProcess(Account account, decimal amount, string description = "", bool isNewOpening = false, TransactionType transactionType = TransactionType.Deposit, bool preApproval = true);

    }

}
