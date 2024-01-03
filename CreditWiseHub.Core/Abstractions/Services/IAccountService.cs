using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Transactions;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IAccountService
    {
        Task<Response<AccountInfoDto>> CreateByUserName(string username, CreateAccountDto createAccountDto);
        Task<Response<RecipientAccountInfoDto>> CheckAccountByAccountNumber(string accountNumber);
        Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amount);
        Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto);
        Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto);
        Task<Response<TransactionStatusDto>> ExternalTransfer(string accountNumber, MoneyExternalTransferDto transferDto);
        Task<Response<AccountInfoDto>> GetAccountInfoByAccountNumber(string accountNumber);
        Task<Response<UserAccountsInfoDto>> GetUserAccountsByAccountNumber(string username);
        Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId);
        Task<Response<NoDataDto>> UpdateAccountAsync(string accountNumber, UpdateAccountDto updateAccountDto);
        Task<Response<AccountHistoryDto>> GetAccountHistoryByAccountNumber(string accountNumber);
    }
}
