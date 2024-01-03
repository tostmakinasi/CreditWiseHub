using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Service.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CreditWiseHub.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGenericRepository<AccountType, int> _accountTypeRepository;
        private readonly ITransactionService _transactionService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IGenericRepository<UserTransactionLimit, string> _userTransactionLimitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IUnitOfWork unitOfWork, UserManager<UserApp> userManager, IGenericRepository<AccountType, int> accountTypeRepository, ITransactionService transactionService, IGenericRepository<UserTransactionLimit, string> userTransactionLimitRepository)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _accountTypeRepository = accountTypeRepository;
            _transactionService = transactionService;
            _userTransactionLimitRepository = userTransactionLimitRepository;
        }

        public async Task<Response<AccountInfoDto>> CreateByUserName(string userName, CreateAccountDto createAccountDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<AccountInfoDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var accountType = await _accountTypeRepository.GetByIdAsync(createAccountDto.AccountTypeId);

            if (accountType is null)
                return Response<AccountInfoDto>.Fail("Account Type not found", HttpStatusCode.NotFound, true);

            if (!CheckOpeningBalance(createAccountDto.OpeningBalance, accountType.MinimumOpeningBalance))
                return Response<AccountInfoDto>.Fail("Account opening amount does not meet the minimum account opening amount.", HttpStatusCode.BadRequest, true);

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
                await _transactionService.AddDepositWithdrawalProcess(account, createAccountDto.OpeningBalance);

            account.Balance = createAccountDto.OpeningBalance;
            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            var accountInfoDto = _mapper.Map<AccountInfoDto>(account);

            return Response<AccountInfoDto>.Success(accountInfoDto, HttpStatusCode.Created);
        }

        public async Task<Response<RecipientAccountInfoDto>> CheckAccountByAccountNumber(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            var user = await _userManager.FindByIdAsync(account.UserAppId);

            if (user is null)
                return Response<RecipientAccountInfoDto>.Fail("User not found", HttpStatusCode.InternalServerError, false);

            RecipientAccountInfoDto info = new()
            {
                AccountId = account.Id.ToString(),
                OwnerFullName = GenerateUserNameWithCensor($"{user.Name} {user.Surname}")
            };

            return Response<RecipientAccountInfoDto>.Success(info, HttpStatusCode.OK);
        }

        public async Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto createDepositDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            await _unitOfWork.BeginTransactionAsync();

            await _transactionService.AddDepositWithdrawalProcess(account, createDepositDto.Amount);

            account.Balance += createDepositDto.Amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.Accepted);
        }

        public async Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(account.UserAppId);

            var checkLimits = await CheckUserLimitationFotWithdraw(amountDto.Amount, account, userLimitation);

            if (!checkLimits.result)
                return Response<AccountLastInfoDto>.Fail(checkLimits.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();
            await _transactionService.AddDepositWithdrawalProcess(account, amountDto.Amount,false, Core.Enums.TransactionType.Withdraw, true);

            account.Balance -= amountDto.Amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            userLimitation.DailyTransactionAmount += amountDto.Amount;

            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.Accepted);
        }

        public async Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto)
        {
            var senderAccount = await GetValidAccountByAccountNumber(accountNumber);

            var receiverAccount = await GetValidAccountByAccountNumber(transferDto.AccountInformation.AccountNumber);

            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(senderAccount.UserAppId);

            var checkLimitation = await CheckUserLimitationFotWithdraw(transferDto.Amount, senderAccount, userLimitation);

            if (!checkLimitation.result)
                return Response<TransactionStatusDto>.Fail(checkLimitation.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();

            var transactionResponse = await _transactionService.CreateInternalTransaction(receiverAccount, senderAccount, transferDto);

            senderAccount.Balance -= transferDto.Amount;
            receiverAccount.Balance += transferDto.Amount;
            senderAccount.UpdatedDate = DateTime.UtcNow;
            receiverAccount.UpdatedDate = DateTime.UtcNow;

            userLimitation.DailyTransactionAmount += transferDto.Amount;

            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionResponse.Data, HttpStatusCode.Accepted);
        }

        public async Task<Response<TransactionStatusDto>> ExternalTransfer(string accountNumber, MoneyExternalTransferDto transferDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            if (transferDto.TransferType == MoneyTransferType.Outgoing)
                return await ExternalOutGoingTransfer(account, transferDto);

            return await ExternalInComingTransfer(account, transferDto);
        }

        private async Task<Response<TransactionStatusDto>> ExternalOutGoingTransfer(Account account, MoneyExternalTransferDto transferDto)
        {
            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(account.UserAppId);
            var checkLimitation = await CheckUserLimitationFotWithdraw(transferDto.Amount, account, userLimitation);

            if (!checkLimitation.result)
                return Response<TransactionStatusDto>.Fail(checkLimitation.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();

            await SaveExternalAccount(transferDto.AccountInformation);

            var transactionResponse = await _transactionService.CreateExternalTransaction(account, transferDto);

            account.Balance -= transferDto.Amount;
            account.UpdatedDate = DateTime.UtcNow;
            userLimitation.DailyTransactionAmount += transferDto.Amount;

            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionResponse.Data, HttpStatusCode.Accepted);
        }

        private async Task<Response<TransactionStatusDto>> ExternalInComingTransfer(Account account, MoneyExternalTransferDto transferDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            await SaveExternalAccount(transferDto.AccountInformation);

            var transactionResponse = await _transactionService.CreateExternalTransaction(account, transferDto);

            account.Balance += transferDto.Amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

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

        public async Task<Response<UserAccountsInfoDto>> GetUserAccountsByAccountNumber(string username)
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

        #region HelperMethods
        private bool CheckOpeningBalance(decimal openingBalance, decimal minimumOpeningBalance)
        {
            if (openingBalance >= minimumOpeningBalance)
                return true;

            return false;
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
        private string GenerateUserNameWithCensor(string userName)
        {
            string result = "";

            string[] words = userName.Split(' ');

            foreach (var word in words)
            {
                if (word.Length > 1)
                {
                    result += $"{word.Substring(0, 1)}{new string('*', word.Length - 1)} ";
                }
                else
                {
                    result += $"{word} ";
                }
            }

            return result.Trim();
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

        public async Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);

            if (account is null)
                throw new BadRequestException("Account not foud");

            return account.UserAppId == userId;
        }
        #endregion

    }
}
