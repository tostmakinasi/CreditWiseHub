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
    /// <inheritdoc/>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGenericRepository<AccountType, int> _accountTypeRepository;
        private readonly ITransactionService _transactionService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GuidHelper _guidHelper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IUnitOfWork unitOfWork, UserManager<UserApp> userManager, IGenericRepository<AccountType, int> accountTypeRepository, ITransactionService transactionService, GuidHelper guidHelper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _accountTypeRepository = accountTypeRepository;
            _transactionService = transactionService;
            _guidHelper = guidHelper;
        }

        public async Task<Response<AccountInfoDto>> CreateByUserName(string userName, CreateAccountDto createAccountDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<AccountInfoDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var accountType = await _accountTypeRepository.GetByIdAsync(createAccountDto.AccountTypeId);

            if (accountType is null)
                return Response<AccountInfoDto>.Fail("Account Type not found", HttpStatusCode.BadRequest, true);

            if (createAccountDto.OpeningBalance < accountType.MinimumOpeningBalance)
                return Response<AccountInfoDto>.Fail("Account opening amount must greater or equal to minimum account opening amount.", HttpStatusCode.BadRequest, true);

            if(createAccountDto.AccountTypeId == 1 && await _accountRepository.UserHaveDefaultAccountAnyAsync(x => x.AccountTypeId == 1 && x.IsActive && x.UserAppId == user.Id))
                return Response<AccountInfoDto>.Fail("You already have a default account", HttpStatusCode.BadRequest, true);

            var account = _mapper.Map<Account>(createAccountDto);

            account.UserAppId = user.Id;
            account.UserApp = user;
            account.AccountNumber = await GenerateAccountNumberAsync();
            account.AccountTypeId = accountType.Id;
            account.AccountType = accountType;
            account.CreatedDate = DateTime.UtcNow;
            account.IsActive = true;
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

        /// <inheritdoc/>
        public async Task<Response<RecipientAccountInfoDto>> CheckAccountByAccountNumber(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            var info = _mapper.Map<RecipientAccountInfoDto>(account);

            return Response<RecipientAccountInfoDto>.Success(info, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto createDepositDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            await _unitOfWork.BeginTransactionAsync();

                await _transactionService.AddDepositWithdrawalProcess(account, createDepositDto.Amount);
                await BalanceTransactionAsync(account, createDepositDto.Amount, BalanceTransactionType.Increase);

            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto)
        {
            var account = await GetValidAccountWithUserAndUserLimitsByAccountNumber(accountNumber);

            var checkLimits = await CheckUserLimitationFotWithdraw(amountDto.Amount, account, account.UserApp.UserTransactionLimit);

            if (!checkLimits.result)
                return Response<AccountLastInfoDto>.Fail(checkLimits.error, HttpStatusCode.BadRequest, false);

            await _unitOfWork.BeginTransactionAsync();
                await _transactionService.AddDepositWithdrawalProcess(account, amountDto.Amount, "", false,TransactionType.Withdraw, true);

                await BalanceTransactionAsync(account, amountDto.Amount, BalanceTransactionType.Decrease);
                await UpdateUserTransactionLimitsAsync(account.UserApp.UserTransactionLimit, amountDto.Amount);
            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<PaymentProcessResultDto>> WithdrawMoneyForAutomaticPayment(PaymentProcessDto dto)
        {
            var account = await _accountRepository.GetUserDefaultAccountByUserIdWithUserAndUserLimits(dto.UserId);

            if (account is null)
                return Response<PaymentProcessResultDto>.Fail("Account not found", HttpStatusCode.NotFound, true);

            var checkLimits = await CheckUserLimitationFotWithdraw(dto.PaymentAmount, account, account.UserApp.UserTransactionLimit);

            if (!checkLimits.result)
                return Response<PaymentProcessResultDto>.Fail(checkLimits.error, HttpStatusCode.BadRequest, false);

            await _unitOfWork.BeginTransactionAsync();
                var transactionResult = await _transactionService.AddDepositWithdrawalProcess(account, dto.PaymentAmount, dto.Description, false, TransactionType.Withdraw, true);

                await BalanceTransactionAsync(account,dto.PaymentAmount, BalanceTransactionType.Decrease);
                await UpdateUserTransactionLimitsAsync(account.UserApp.UserTransactionLimit, dto.PaymentAmount);

            await _unitOfWork.TransactionCommitAsync();

            var result = _mapper.Map<PaymentProcessResultDto>(transactionResult.data);

            return Response<PaymentProcessResultDto>.Success(result, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto)
        {
            var senderAccount = await GetValidAccountWithUserAndUserLimitsByAccountNumber(accountNumber);

            var receiverAccount = await GetValidAccountByAccountNumber(transferDto.AccountInformation.AccountNumber);

            var userNameValidation = $"{receiverAccount.UserApp.Name} {receiverAccount.UserApp.Surname}".ToLower() == transferDto.AccountInformation.AccountHolderFullName.ToLower().Trim();

            if (!userNameValidation)
                return Response<TransactionStatusDto>.Fail("Account Holder Name doesn't match", HttpStatusCode.BadRequest, false);

            var checkLimitation = await CheckUserLimitationFotWithdraw(transferDto.Amount, senderAccount, senderAccount.UserApp.UserTransactionLimit);

            if (!checkLimitation.result)
                return Response<TransactionStatusDto>.Fail(checkLimitation.error, HttpStatusCode.BadRequest, false);

            await _unitOfWork.BeginTransactionAsync();

                var transactionResponse = await _transactionService.CreateInternalTransaction(receiverAccount, senderAccount, transferDto);

                await BalanceTransactionAsync(senderAccount, transferDto.Amount, BalanceTransactionType.Decrease);
                await UpdateUserTransactionLimitsAsync(senderAccount.UserApp.UserTransactionLimit, transferDto.Amount);
                await BalanceTransactionAsync(receiverAccount, transferDto.Amount, BalanceTransactionType.Increase);
 
            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionResponse.data, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<TransactionStatusDto>> ExternalTransfer(string accountNumber, MoneyExternalTransferDto transferDto)
        {
            var account = await GetValidAccountWithUserAndUserLimitsByAccountNumber(accountNumber);

            var balanceTransactionType = transferDto.TransferType == MoneyTransferType.Outgoing ? BalanceTransactionType.Decrease : BalanceTransactionType.Increase;
            await _unitOfWork.BeginTransactionAsync();
            
            if (transferDto.TransferType == MoneyTransferType.Outgoing)
            {
                var checkLimitation = await CheckUserLimitationFotWithdraw(transferDto.Amount, account, account.UserApp.UserTransactionLimit);

                if (!checkLimitation.result)
                    return Response<TransactionStatusDto>.Fail(checkLimitation.error, HttpStatusCode.BadRequest, false);

                await UpdateUserTransactionLimitsAsync(account.UserApp.UserTransactionLimit, transferDto.Amount);
            }
            await BalanceTransactionAsync(account, transferDto.Amount, balanceTransactionType);
            await SaveExternalAccount(transferDto.AccountInformation);

            var transactionResponse = await _transactionService.CreateExternalTransaction(account, transferDto);
            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionResponse.data, HttpStatusCode.OK);

        }

        /// <inheritdoc/>
        private async Task SaveExternalAccount(AffectedExternalAccountDto accountDto)
        {
            var externalAccount = _mapper.Map<ExternalAccountInformation>(accountDto);

            await _accountRepository.AddExternalAccount(externalAccount);
            await _unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task<Response<AccountInfoDto>> GetAccountInfoByAccountNumber(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            var accountInfoDto = _mapper.Map<AccountInfoDto>(account);

            return Response<AccountInfoDto>.Success(accountInfoDto, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<Response<NoDataDto>> UpdateAccountAsync(string accountNumber, UpdateAccountDto updateAccountDto)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);

            account.Name = updateAccountDto.Name is not null ? updateAccountDto.Name : account.Name;
            account.Description = updateAccountDto.Description is not null ? updateAccountDto.Description : account.Description;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<AccountHistoryDto>> GetAccountHistoryByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountsWithTransactionsByAccountNumber(accountNumber);
            if (account is null)
                return Response<AccountHistoryDto>.Fail($"Account ({accountNumber}) not found", HttpStatusCode.NotFound, true);

            var accountHistoryDto = _mapper.Map<AccountHistoryDto>(account);
            return Response<AccountHistoryDto>.Success(accountHistoryDto, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<TransactionStatusDto>> TransferCreditToAccountByUserId(string userId, decimal creditAmount)
        {
            var userDefaultAccount = await _accountRepository.GetUserDefaultAccountByUserIdWithUserAndUserLimits(userId);
            await _unitOfWork.BeginTransactionAsync();

            var transactionStatusResponse = await _transactionService.AddDepositWithdrawalProcess(userDefaultAccount, creditAmount, "Onaylanan Kredi Tutarı Hesaba Aktarımı");

            await BalanceTransactionAsync(userDefaultAccount, creditAmount, BalanceTransactionType.Increase);

            await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionStatusResponse.data, HttpStatusCode.OK);
        }

        /// <inheritdoc/>
        public async Task<Response<NoDataDto>> CloseAccount(string accountNumber)
        {
            var account = await GetValidAccountByAccountNumber(accountNumber);
            var defaultAccount = await _accountRepository.GetUserDefaultAccountByUserIdWithUserAndUserLimits(account.UserAppId);

            if (account.AccountTypeId == 1)
                return Response<NoDataDto>.Fail("Default account cannot be closed", HttpStatusCode.BadRequest, true);

            if (account.Balance > 0 && defaultAccount is null)
                return Response<NoDataDto>.Fail("Account Balance must be 0", HttpStatusCode.BadRequest, true);
            else if(account.Balance == 0 && defaultAccount is null)
                return Response<NoDataDto>.Fail("User Default Account Not Found", HttpStatusCode.NotFound, true);

            await _unitOfWork.BeginTransactionAsync();

            await _transactionService.AddDepositWithdrawalProcess(account, account.Balance, $" Hesabın kapanması işlemi ile aktarılan tutar", false, TransactionType.Withdraw, true);

            await _transactionService.AddDepositWithdrawalProcess(defaultAccount, account.Balance, $"{account.AccountNumber} numaralı hesabın kapanması işlemi ile aktarılan tutar", false, TransactionType.Deposit, true);

            await BalanceTransactionAsync(defaultAccount, account.Balance, BalanceTransactionType.Increase);
            await BalanceTransactionAsync(account, account.Balance, BalanceTransactionType.Decrease);

            account.IsActive = false;
            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.NoContent);
        }

        /// <inheritdoc/>
        public async Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);

            if (account is null)
                throw new BadRequestException("Account not found");

            return account.UserAppId == userId;
        }

        /// <inheritdoc/>
        public async Task<Response<AccountBalanceInfoDto>> GetAccountBalanceByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);
            if (account is null)
                return Response<AccountBalanceInfoDto>.Fail($"Account({accountNumber}) not found", HttpStatusCode.NotFound, true);

            return Response<AccountBalanceInfoDto>.Success(new AccountBalanceInfoDto { Balance = account.Balance }, HttpStatusCode.OK);
        }

        /// <summary>
        /// Updates the account balance based on the specified transaction type (increase or decrease).
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <param name="amount">The amount to update the balance.</param>
        /// <param name="transactionType">The type of transaction (increase or decrease).</param>
        private async Task BalanceTransactionAsync(Account account, decimal amount, BalanceTransactionType transactionType)
        {

            account.Balance = transactionType == BalanceTransactionType.Increase?account.Balance + amount : account.Balance - amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

        }

        /// <summary>
        /// Updates the user's daily transaction limits after a successful transaction.
        /// </summary>
        /// <param name="userLimit">The user's transaction limits.</param>
        /// <param name="amount">The transaction amount to update the limits.</param>
        private async Task UpdateUserTransactionLimitsAsync(UserTransactionLimit userLimit, decimal amount)
        {
            userLimit.DailyTransactionAmount += amount;
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Checks the user's limitations for a withdrawal transaction and returns the result.
        /// </summary>
        /// <param name="amount">The withdrawal amount to check.</param>
        /// <param name="account">The user's account.</param>
        /// <param name="userLimitation">The user's transaction limits.</param>
        /// <returns>A tuple indicating the result (true if allowed, false if not) and an error message if any.</returns>
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

        /// <summary>
        /// Retrieves a valid account based on its account number, throwing an exception if not found.
        /// </summary>
        /// <param name="accountNumber">The account number to search for.</param>
        /// <returns>The valid account associated with the provided account number.</returns>
        private async Task<Account> GetValidAccountByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);
            if (account is null)
                throw new NotFoundException($"Account ({accountNumber}) not found");
            return account;
        }

        /// <summary>
        /// Retrieves a valid account with user and user limits based on its account number, throwing an exception if not found.
        /// </summary>
        /// <param name="accountNumber">The account number to search for.</param>
        /// <returns>The valid account with user and user limits associated with the provided account number.</returns>
        private async Task<Account> GetValidAccountWithUserAndUserLimitsByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumberWithUserAndUserLimits(accountNumber);
            if (account is null)
                throw new NotFoundException($"Account ({accountNumber}) not found");
            return account;
        }

        /// <summary>
        /// Generates a unique account number and ensures its uniqueness in the repository.
        /// </summary>
        /// <returns>A unique account number.</returns>
        private async Task<string> GenerateAccountNumberAsync()
        {
            string accountNumber = "";
            var accountNumberCheck = true;
            do
            {
                accountNumber = _guidHelper.GetAccountNumberBaseGuid();
                accountNumberCheck = await _accountRepository.AnyAsync(x => x.AccountNumber == accountNumber);

            } while (accountNumberCheck);

            return accountNumber;
        }

        
    }
}
