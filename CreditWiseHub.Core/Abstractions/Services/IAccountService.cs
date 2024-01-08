using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IAccountService
    {
        Task<Response<AccountInfoDto>> CreateByUserName(string username, CreateAccountDto createAccountDto);
        Task<Response<RecipientAccountInfoDto>> CheckAccountByAccountNumber(string accountNumber);
        Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amount);
        Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto, string description = "");
        Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto);
        Task<Response<TransactionStatusDto>> ExternalTransfer(string accountNumber, MoneyExternalTransferDto transferDto);
        Task<Response<AccountInfoDto>> GetAccountInfoByAccountNumber(string accountNumber);
        Task<Response<UserAccountsInfoDto>> GetUserAccountsByUsername(string username);
        Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId);
        Task<Response<NoDataDto>> UpdateAccountAsync(string accountNumber, UpdateAccountDto updateAccountDto);
        Task<Response<AccountHistoryDto>> GetAccountHistoryByAccountNumber(string accountNumber);
        Task<Response<TransactionStatusDto>> TransferCreditToAccountByUserId(string userId, decimal creditAmount);
        Task<Response<PaymentProcessResultDto>> WithdrawMoneyForAutomaticPayment(PaymentProcessDto dto);
        Task<Response<NoDataDto>> CloseAccount(string accountNumber);
    }
}
