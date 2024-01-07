using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface ITransactionService
    {
        Task<Response<TransactionStatusDto>> CreateInternalTransaction(Account receiverAccount, Account senderAccount, MoneyTransferDto transferDto);
        Task<Response<TransactionStatusDto>> CreateExternalTransaction(Account account, MoneyExternalTransferDto transferDto);
        Task<Response<TransactionStatusDto>> CheckTransactionStatus(long transactionId);

        Task<Response<TransactionStatusDto>> AddDepositWithdrawalProcess(Account account, decimal amount, string description ="", bool isNewOpening = false, TransactionType transactionType = TransactionType.Deposit, bool preApproval = true);

    }
}
