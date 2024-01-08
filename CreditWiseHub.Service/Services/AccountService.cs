using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using CreditWiseHub.Service.Exceptions;
using CreditWiseHub.Service.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;

namespace CreditWiseHub.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGenericRepository<AccountType, int> _accountTypeRepository;
        private readonly ITransactionService _transactionService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IUnitOfWork unitOfWork, UserManager<UserApp> userManager, IGenericRepository<AccountType, int> accountTypeRepository, ITransactionService transactionService)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _accountTypeRepository = accountTypeRepository;
            _transactionService = transactionService;
        }

        public async Task<Response<AccountInfoDto>> CreateByUserName(string userName, CreateAccountDto createAccountDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<AccountInfoDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var accountType = await _accountTypeRepository.GetByIdAsync(createAccountDto.AccountTypeId);

            if (accountType is null)
                return Response<AccountInfoDto>.Fail("Account Type not found", HttpStatusCode.NotFound, true);

            if (createAccountDto.OpeningBalance < accountType.MinimumOpeningBalance)
                return Response<AccountInfoDto>.Fail("Account opening amount must greater or equal to minimum account opening amount.", HttpStatusCode.BadRequest, true);

            if(createAccountDto.AccountTypeId == 1 && await _accountRepository.Where(x => x.AccountTypeId == 1 && x.IsActive && x.UserAppId == user.Id).AnyAsync())
                return Response<AccountInfoDto>.Fail("You already have a default account", HttpStatusCode.NotAcceptable, true);

            var account = _mapper.Map<Account>(createAccountDto);

            account.UserAppId = user.Id;
            account.UserApp = user;
            account.AccountNumber = await GenerateAccountNumberAsync();
            account.AccountTypeId = accountType.Id;
            account.AccountType = accountType;
            account.CreatedDate = DateTime.UtcNow;
            await _unitOfWork.BeginTransactionAsync();

            await _accountRepository.AddAsync(account);
            await _unitOfWork.CommitAsync();

            if (createAccountDto.OpeningBalance > 0)
                await _transactionService.AddDepositWithdrawalProcess(account, createAccountDto.OpeningBalance, "Hesap Açılışı İçin Gereken Tutar Yatırma İşlemi");

            account.Balance = createAccountDto.OpeningBalance;
            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            var accountInfoDto = _mapper.Map<AccountInfoDto>(account);

            return Response<AccountInfoDto>.Success(accountInfoDto, HttpStatusCode.Created);
        }

        public async Task<Response<RecipientAccountInfoDto>> CheckAccountByAccountNumber(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            var info = _mapper.Map<RecipientAccountInfoDto>(account);

            return Response<RecipientAccountInfoDto>.Success(info, HttpStatusCode.OK);
        }

        public async Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto createDepositDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            await _unitOfWork.BeginTransactionAsync();

                await _transactionService.AddDepositWithdrawalProcess(account, createDepositDto.Amount);
                await BalanceTransactionAsync(account, account.Balance,BalanceTransactionType.Increase);

            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.Accepted);
        }

        public async Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto, string description = "")
        {
            var account = await GetValidAccountWithUserAndUserLimitsByAccountNumber(accountNumber);

            var checkLimits = await CheckUserLimitationFotWithdraw(amountDto.Amount, account, account.UserApp.UserTransactionLimit);

            if (!checkLimits.result)
                return Response<AccountLastInfoDto>.Fail(checkLimits.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();
                await _transactionService.AddDepositWithdrawalProcess(account, amountDto.Amount, description, false,TransactionType.Withdraw, true);

                await BalanceTransactionAsync(account, amountDto.Amount, BalanceTransactionType.Decrease);
                await UpdateUserTransactionLimitsAsync(account.UserApp.UserTransactionLimit, amountDto.Amount);
            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.Accepted);
        }

        public async Task<Response<PaymentProcessResultDto>> WithdrawMoneyForAutomaticPayment(PaymentProcessDto dto)
        {
            var account = await _accountRepository.GetUserDefaultAccountByAccountNumberWithUserAndUserLimits(dto.UserId);

            if (account is null)
                return Response<PaymentProcessResultDto>.Fail("Account not found", HttpStatusCode.NotFound, true);

            var checkLimits = await CheckUserLimitationFotWithdraw(dto.PaymentAmount, account, account.UserApp.UserTransactionLimit);

            if (!checkLimits.result)
                return Response<PaymentProcessResultDto>.Fail(checkLimits.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();
                var transactionResult = await _transactionService.AddDepositWithdrawalProcess(account, dto.PaymentAmount, dto.Description, false, TransactionType.Withdraw, true);

                await BalanceTransactionAsync(account,dto.PaymentAmount, BalanceTransactionType.Decrease);
                await UpdateUserTransactionLimitsAsync(account.UserApp.UserTransactionLimit, dto.PaymentAmount);

            await _unitOfWork.TransactionCommitAsync();

            var result = _mapper.Map<PaymentProcessResultDto>(transactionResult.Data);

            return Response<PaymentProcessResultDto>.Success(result, HttpStatusCode.OK);
        }

        public async Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto)
        {
            var senderAccount = await GetValidAccountWithUserAndUserLimitsByAccountNumber(accountNumber);

            var receiverAccount = await GetValidAccountByAccountNumber(transferDto.AccountInformation.AccountNumber);

            var userNameValidation = $"{receiverAccount.UserApp.Name} {receiverAccount.UserApp.Surname}".ToLower() == transferDto.AccountInformation.AccountHolderFullName.ToLower().Trim();

            if (!userNameValidation)
                return Response<TransactionStatusDto>.Fail("Account Holder Name doesn't match", HttpStatusCode.BadRequest, false);

            var checkLimitation = await CheckUserLimitationFotWithdraw(transferDto.Amount, senderAccount, senderAccount.UserApp.UserTransactionLimit);

            if (!checkLimitation.result)
                return Response<TransactionStatusDto>.Fail(checkLimitation.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();

                var transactionResponse = await _transactionService.CreateInternalTransaction(receiverAccount, senderAccount, transferDto);

                await BalanceTransactionAsync(senderAccount, transferDto.Amount, BalanceTransactionType.Decrease);
                await UpdateUserTransactionLimitsAsync(senderAccount.UserApp.UserTransactionLimit, transferDto.Amount);
                await BalanceTransactionAsync(receiverAccount, transferDto.Amount, BalanceTransactionType.Increase);
 
            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionResponse.Data, HttpStatusCode.Accepted);
        }

        public async Task<Response<TransactionStatusDto>> ExternalTransfer(string accountNumber, MoneyExternalTransferDto transferDto)
        {
            var account = await GetValidAccountWithUserAndUserLimitsByAccountNumber(accountNumber);

            var balanceTransactionType = transferDto.TransferType == MoneyTransferType.Outgoing ? BalanceTransactionType.Decrease : BalanceTransactionType.Increase;
            await _unitOfWork.BeginTransactionAsync();
            
            if (transferDto.TransferType == MoneyTransferType.Outgoing)
            {
                var checkLimitation = await CheckUserLimitationFotWithdraw(transferDto.Amount, account, account.UserApp.UserTransactionLimit);

                if (!checkLimitation.result)
                    return Response<TransactionStatusDto>.Fail(checkLimitation.error, HttpStatusCode.NotAcceptable, false);

                await UpdateUserTransactionLimitsAsync(account.UserApp.UserTransactionLimit, transferDto.Amount);
            }
            await BalanceTransactionAsync(account, transferDto.Amount, balanceTransactionType);
            await SaveExternalAccount(transferDto.AccountInformation);

            var transactionResponse = await _transactionService.CreateExternalTransaction(account, transferDto);
            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionResponse.Data, HttpStatusCode.Accepted);

        }

        private async Task SaveExternalAccount(AffectedExternalAccountDto accountDto)
        {
            var externalAccount = _mapper.Map<ExternalAccountInformation>(accountDto);

            await _accountRepository.AddExternalAccount(externalAccount);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Response<AccountInfoDto>> GetAccountInfoByAccountNumber(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            var accountInfoDto = _mapper.Map<AccountInfoDto>(account);

            return Response<AccountInfoDto>.Success(accountInfoDto, HttpStatusCode.OK);
        }

        public async Task<Response<UserAccountsInfoDto>> GetUserAccountsByUsername(string username)
        {
            var accounts = await _accountRepository.GetAccountsByUsername(username);

            if (accounts is null || accounts.Count <= 0)
                return Response<UserAccountsInfoDto>.Fail("User Accounts not found", HttpStatusCode.NotFound, true);

            var accountInfoDtoList = _mapper.Map<List<AccountInfoDto>>(accounts);

            UserAccountsInfoDto userAccountsInfo = new()
            {
                TotalBalance = accounts.Sum(x => x.Balance),
                UserAccounts = accountInfoDtoList
            };

            return Response<UserAccountsInfoDto>.Success(userAccountsInfo, HttpStatusCode.OK);

        }

        public async Task<Response<NoDataDto>> UpdateAccountAsync(string accountNumber, UpdateAccountDto updateAccountDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            account.Name = updateAccountDto.Name is not null ? updateAccountDto.Name : account.Name;
            account.Description = updateAccountDto.Description is not null ? updateAccountDto.Description : account.Description;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        public async Task<Response<AccountHistoryDto>> GetAccountHistoryByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountsWithTransactionsByAccountNumber(accountNumber);
            if (account is null)
                return Response<AccountHistoryDto>.Fail($"Account ({accountNumber}) not found", HttpStatusCode.NotFound, true);

            var accountHistoryDto = _mapper.Map<AccountHistoryDto>(account);
            return Response<AccountHistoryDto>.Success(accountHistoryDto, HttpStatusCode.OK);
        }

        public async Task<Response<TransactionStatusDto>> TransferCreditToAccountByUserId(string userId, decimal creditAmount)
        {
            var userDefaultAccount = await _accountRepository.GetUserDefaultAccountByAccountNumberWithUserAndUserLimits(userId);
            await _unitOfWork.BeginTransactionAsync();

            var transactionStatusResponse = await _transactionService.AddDepositWithdrawalProcess(userDefaultAccount, creditAmount, "Onaylanan Kredi Tutarı Hesaba Aktarımı");

            await BalanceTransactionAsync(userDefaultAccount, creditAmount, BalanceTransactionType.Increase);

            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionStatusResponse.Data, HttpStatusCode.Accepted);
        }

        public async Task<Response<NoDataDto>> CloseAccount(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            if (account.Balance > 0)
                return Response<NoDataDto>.Fail("Account Balance must be 0", HttpStatusCode.NotAcceptable, true);
            if(account.AccountTypeId == 1)
                return Response<NoDataDto>.Fail("Default account cannot be closed", HttpStatusCode.NotAcceptable, true);

            account.IsActive = false;
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(HttpStatusCode.NoContent);
        }

        public async Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);

            if (account is null)
                throw new BadRequestException("Account not found");

            return account.UserAppId == userId;
        }


        private async Task BalanceTransactionAsync(Account account, decimal amount, BalanceTransactionType transactionType)
        {

            account.Balance = transactionType == BalanceTransactionType.Increase?account.Balance + amount : account.Balance - amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

        }

        private async Task UpdateUserTransactionLimitsAsync(UserTransactionLimit userLimit, decimal amount)
        {
            userLimit.DailyTransactionAmount += amount;
            await _unitOfWork.CommitAsync();
        }

        private async Task<(bool result, string error)> CheckUserLimitationFotWithdraw(decimal amount, Account account, UserTransactionLimit userLimitation)
        {
            if (account.Balance < amount)
                return (false, $"Account({account.AccountNumber}) balance is insufficient");

            if (userLimitation.LastProcessDate.Date < DateTime.UtcNow.Date)
            {
                userLimitation.LastProcessDate = DateTime.UtcNow.Date;
                userLimitation.DailyTransactionAmount = 0;

                await _unitOfWork.CommitAsync();
            }

            if (amount > userLimitation.InstantTransactionLimit)
                return (false, $"Account instant transaction limit is insufficient");

            if (amount + userLimitation.DailyTransactionAmount > userLimitation.DailyTransactionLimit)
                return (false, $"Account monthly transaction limit is insufficient");

            return (true, "");
        }

        private async Task<Account> GetValidAccountByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);
            if (account is null)
                throw new NotFoundException($"Account ({accountNumber}) not found");
            return account;
        }

        private async Task<Account> GetValidAccountWithUserAndUserLimitsByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumberWithUserAndUserLimits(accountNumber);
            if (account is null)
                throw new NotFoundException($"Account ({accountNumber}) not found");
            return account;
        }

        private async Task<string> GenerateAccountNumberAsync()
        {
            string accountNumber = "";
            var accountNumberCheck = true;
            do
            {
                accountNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                accountNumberCheck = await _accountRepository.AnyAsync(x => x.AccountNumber == accountNumber);

            } while (accountNumberCheck);

            return accountNumber;
        }

        private bool CheckOpeningBalance(decimal openingBalance, decimal minimumOpeningBalance)
        {
            if (openingBalance >= minimumOpeningBalance)
                return true;

            return false;
        }

    }
}
