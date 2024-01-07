﻿using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
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
        private readonly IGenericRepository<UserTransactionLimit, string> _userTransactionLimitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AccountHelper _helper; 

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IUnitOfWork unitOfWork, UserManager<UserApp> userManager, IGenericRepository<AccountType, int> accountTypeRepository, ITransactionService transactionService, IGenericRepository<UserTransactionLimit, string> userTransactionLimitRepository, AccountHelper helper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _accountTypeRepository = accountTypeRepository;
            _transactionService = transactionService;
            _userTransactionLimitRepository = userTransactionLimitRepository;
            _helper = helper;
        }

        public async Task<Response<AccountInfoDto>> CreateByUserName(string userName, CreateAccountDto createAccountDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<AccountInfoDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var accountType = await _accountTypeRepository.GetByIdAsync(createAccountDto.AccountTypeId);

            if (accountType is null)
                return Response<AccountInfoDto>.Fail("Account Type not found", HttpStatusCode.NotFound, true);

            if (!_helper.CheckOpeningBalance(createAccountDto.OpeningBalance, accountType.MinimumOpeningBalance))
                return Response<AccountInfoDto>.Fail("Account opening amount does not meet the minimum account opening amount.", HttpStatusCode.BadRequest, true);



            if(createAccountDto.AccountTypeId == 1 && await _accountRepository.Where(x => x.AccountTypeId == 1 && x.IsActive && x.UserAppId == user.Id).AnyAsync())
                return Response<AccountInfoDto>.Fail("You already have a default account", HttpStatusCode.NotAcceptable, true);

            var account = _mapper.Map<Account>(createAccountDto);

            account.UserAppId = user.Id;
            account.UserApp = user;
            account.AccountNumber = await _helper.GenerateAccountNumberAsync();
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
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

            var user = await _userManager.FindByIdAsync(account.UserAppId);

            if (user is null)
                return Response<RecipientAccountInfoDto>.Fail("User not found", HttpStatusCode.InternalServerError, false);

            RecipientAccountInfoDto info = new()
            {
                AccountId = account.Id.ToString(),
                OwnerFullName = _helper.GenerateUserNameWithCensor($"{user.Name} {user.Surname}")
            };

            return Response<RecipientAccountInfoDto>.Success(info, HttpStatusCode.OK);
        }

        public async Task<Response<AccountLastInfoDto>> DepositMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto createDepositDto)
        {
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

            await _unitOfWork.BeginTransactionAsync();

            await _transactionService.AddDepositWithdrawalProcess(account, createDepositDto.Amount);

            account.Balance += createDepositDto.Amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.Accepted);
        }

        public async Task<Response<AccountLastInfoDto>> WithdrawMoneyByAccountNumber(string accountNumber, MoneyProcessAmountDto amountDto, string description = "")
        {
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(account.UserAppId);

            var checkLimits = await _helper.CheckUserLimitationFotWithdraw(amountDto.Amount, account, userLimitation);

            if (!checkLimits.result)
                return Response<AccountLastInfoDto>.Fail(checkLimits.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();
            await _transactionService.AddDepositWithdrawalProcess(account, amountDto.Amount, description, false,TransactionType.Withdraw, true);

            account.Balance -= amountDto.Amount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            userLimitation.DailyTransactionAmount += amountDto.Amount;

            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            var lasInfoDto = _mapper.Map<AccountLastInfoDto>(account);

            return Response<AccountLastInfoDto>.Success(lasInfoDto, HttpStatusCode.Accepted);
        }

        public async Task<Response<PaymentProcessResultDto>> WithdrawMoneyForAutomaticPayment(PaymentProcessDto dto)
        {
            var account = await _accountRepository.Where(x => x.UserAppId == dto.UserId && x.AccountTypeId == 1).FirstOrDefaultAsync();

            if (account is null)
                return Response<PaymentProcessResultDto>.Fail("Account not found", HttpStatusCode.NotFound, true);

            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(dto.UserId);

            var checkLimits = await _helper.CheckUserLimitationFotWithdraw(dto.PaymentAmount, account, userLimitation);

            if (!checkLimits.result)
                return Response<PaymentProcessResultDto>.Fail(checkLimits.error, HttpStatusCode.NotAcceptable, false);

            await _unitOfWork.BeginTransactionAsync();
            var transactionResult = await _transactionService.AddDepositWithdrawalProcess(account, dto.PaymentAmount, dto.Description, false, TransactionType.Withdraw, true);

            account.Balance -= dto.PaymentAmount;
            account.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            await _unitOfWork.TransactionCommitAsync();

            PaymentProcessResultDto result = new()
            {
                PaymentDate = transactionResult.Data.TransactionDate,
                TransactionId = transactionResult.Data.TransactionId,

            };

            return Response<PaymentProcessResultDto>.Success(result, HttpStatusCode.OK);
        }

        public async Task<Response<TransactionStatusDto>> InternalMoneyTransfer(string accountNumber, MoneyTransferDto transferDto)
        {
            var senderAccount = await _helper.GetValidAccountByAccountNumber(accountNumber);

            var receiverAccount = await _helper.GetValidAccountByAccountNumber(transferDto.AccountInformation.AccountNumber);

            if($"{receiverAccount.UserApp.Name} {receiverAccount.UserApp.Surname}".ToLower() != transferDto.AccountInformation.AccountHolderFullName.ToLower().Trim())
                return Response<TransactionStatusDto>.Fail("Account Holder Name doesn't match", HttpStatusCode.BadRequest, false);

            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(senderAccount.UserAppId);

            var checkLimitation = await _helper.CheckUserLimitationFotWithdraw(transferDto.Amount, senderAccount, userLimitation);

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
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

            if (transferDto.TransferType == MoneyTransferType.Outgoing)
                return await ExternalOutGoingTransfer(account, transferDto);

            return await ExternalInComingTransfer(account, transferDto);
        }

        private async Task<Response<TransactionStatusDto>> ExternalOutGoingTransfer(Account account, MoneyExternalTransferDto transferDto)
        {
            var userLimitation = await _userTransactionLimitRepository.GetByIdAsync(account.UserAppId);
            var checkLimitation = await _helper.CheckUserLimitationFotWithdraw(transferDto.Amount, account, userLimitation);

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
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

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
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

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
            var userDefaultAccount = await _accountRepository.Where(x => x.AccountTypeId == 1 && x.UserAppId == userId).FirstOrDefaultAsync();
            //await _unitOfWork.BeginTransactionAsync();

            var transactionStatusResponse = await _transactionService.AddDepositWithdrawalProcess(userDefaultAccount, creditAmount, "Onaylanan Kredi Tutarı Hesaba Aktarımı");

            userDefaultAccount.Balance += creditAmount;
            userDefaultAccount.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            //await _unitOfWork.TransactionCommitAsync();

            return Response<TransactionStatusDto>.Success(transactionStatusResponse.Data, HttpStatusCode.Accepted);
        }

        public async Task<Response<NoDataDto>> CloseAccount(string accountNumber)
        {
            var account = await _helper.GetValidAccountByAccountNumber(accountNumber);

            if (account.Balance > 0)
                return Response<NoDataDto>.Fail("Account Balance must be 0", HttpStatusCode.NotAcceptable, true);
            if(account.AccountTypeId == 1)
                return Response<NoDataDto>.Fail("Default account cannot be closed", HttpStatusCode.NotAcceptable, true);

            account.IsActive = false;
            return Response<NoDataDto>.Success(HttpStatusCode.NoContent);
        }

        public async Task<bool> DoesAccountBelongToUserId(string accountNumber, string userId)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);

            if (account is null)
                throw new BadRequestException("Account not foud");

            return account.UserAppId == userId;
        }

    }
}
