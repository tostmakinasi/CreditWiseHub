using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using System.Net;

namespace CreditWiseHub.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IGenericRepository<Transaction, long> _transactionRepository;
        private readonly IGenericRepository<TransactionAffectedAccount, long> _affectedAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IGenericRepository<Transaction, long> genericRepository, IUnitOfWork unitOfWork, IGenericRepository<TransactionAffectedAccount, long> affectedAccountRepository, IMapper mapper)
        {
            _transactionRepository = genericRepository;
            _unitOfWork = unitOfWork;
            _affectedAccountRepository = affectedAccountRepository;
            _mapper = mapper;
        }

        public async Task<Response<TransactionStatusDto>> AddDepositWithdrawalProcess(Account account, decimal amount,string description = "", bool isNewOpening = false, TransactionType transactionType = TransactionType.Deposit, bool preApproval = true)
        {

            var transaction = await CreateTransaction(amount, preApproval);
            var affectedAccount = await CreateAffectedAccount(account, amount, transactionType, transaction);

            if (!string.IsNullOrEmpty(description))
                affectedAccount.Description = description;

            await _affectedAccountRepository.AddAsync(affectedAccount);
            await _unitOfWork.CommitAsync();

            var transactionComplete = _mapper.Map<TransactionStatusDto>(transaction);

            return Response<TransactionStatusDto>.Success(transactionComplete, HttpStatusCode.OK);
        }

        public async Task<Response<TransactionStatusDto>> CheckTransactionStatus(long transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction is null)
                return Response<TransactionStatusDto>.Fail($"Transaction({transactionId}) not found", HttpStatusCode.NotFound, true);

            var transactionComplete = _mapper.Map<TransactionStatusDto>(transaction);

            return Response<TransactionStatusDto>.Success(transactionComplete, HttpStatusCode.OK);
        }

        public async Task<Response<TransactionStatusDto>> CreateExternalTransaction(Account account, MoneyExternalTransferDto transferDto)
        {
            var transaction = await CreateTransaction(transferDto.Amount, true);


            var affectedAccount = await CreateAffectedAccount(account, transferDto.Amount, TransactionType.ExternalTransfer, transaction, (transferDto.TransferType == MoneyTransferType.Incoming), transferDto.AccountInformation.OwnerFullName);

            var affectedExternalAccount = new TransactionAffectedAccount()
            {
                AccountNumber = transferDto.AccountInformation.AccountNumber,
                Description = transferDto.AccountInformation.OwnerFullName,
                IsExternal = true
            };

            await _affectedAccountRepository.AddAsync(affectedAccount);
            await _unitOfWork.CommitAsync();

            affectedExternalAccount.IsReceiverAccount = !affectedAccount.IsReceiverAccount;
            affectedExternalAccount.TransactionId = transaction.Id;

            await _affectedAccountRepository.AddAsync(affectedExternalAccount);
            await _unitOfWork.CommitAsync();

            var transactionComplete = _mapper.Map<TransactionStatusDto>(transaction);

            return Response<TransactionStatusDto>.Success(transactionComplete, HttpStatusCode.OK);
        }

        public async Task<Response<TransactionStatusDto>> CreateInternalTransaction(Account receiverAccount, Account senderAccount, MoneyTransferDto transferDto)
        {
            var transaction = await CreateTransaction(transferDto.Amount, true);

            var affectedReceiverAccount = await CreateAffectedAccount(receiverAccount, transferDto.Amount, TransactionType.ExternalTransfer, transaction, true, transferDto.AccountInformation.AccountHolderFullName);

            var affectedSenderAccount = await CreateAffectedAccount(senderAccount, transferDto.Amount, TransactionType.InternalTransfer, transaction, false);
            //TransactionAffectedAccount affectedReceiverAccount = new()
            //{
            //    IsReceiverAccount = true,
            //    AccountNumber = receiverAccount.AccountNumber,
            //    Description = $"Gelen Para Transferi Gönderen : {transferDto.AccountInformation.AccountHolderFullName}",
            //    BeforeBalance = receiverAccount.Balance,
            //    AfterBalance = receiverAccount.Balance + transferDto.Amount,
            //};



            //TransactionAffectedAccount affectedSenderAccount = new()
            //{
            //    IsReceiverAccount = false,
            //    AccountNumber = transferDto.AccountInformation.AccountNumber,
            //    Description = $"Giden Para Transferi Kime : {transferDto.AccountInformation.AccountHolderFullName}",
            //    BeforeBalance = senderAccount.Balance,
            //    AfterBalance = senderAccount.Balance - transferDto.Amount,
            //};

            await _affectedAccountRepository.AddAsync(affectedReceiverAccount);
            await _affectedAccountRepository.AddAsync(affectedSenderAccount);

            await _unitOfWork.CommitAsync();

            var transactionComplete = _mapper.Map<TransactionStatusDto>(transaction);

            return Response<TransactionStatusDto>.Success(transactionComplete, HttpStatusCode.OK);
        }

        private async Task<TransactionAffectedAccount> CreateAffectedAccount(Account account, decimal amount, TransactionType transactionType, Transaction transaction, bool isReceiverAccount = true, string counterAccountHolder = "")
        {
            var affectedAccount = new TransactionAffectedAccount()
            {
                AccountNumber = account.AccountNumber,
                BeforeBalance = account.Balance,
                IsReceiverAccount = isReceiverAccount,
                TransactionId = transaction.Id
            };

            if (transactionType == TransactionType.Deposit)
            {
                affectedAccount.AfterBalance = account.Balance + amount;
                affectedAccount.Description = "Para Yatırma İşlemi";
            }
            else if (transactionType == TransactionType.Withdraw)
            {
                affectedAccount.AfterBalance = account.Balance - amount;
                affectedAccount.Description = "Para Çekme İşlemi";
            }
            else
            {
                if (isReceiverAccount)
                {
                    affectedAccount.Description = $"Gelen Para Transferi Gönderen : {counterAccountHolder}";
                    affectedAccount.AfterBalance = account.Balance + amount;
                }
                else
                {
                    affectedAccount.Description = $"Giden Para Transferi Kime : {counterAccountHolder}";
                    affectedAccount.AfterBalance = account.Balance - amount;
                }

            }

            return affectedAccount;
        }

        private async Task<Transaction> CreateTransaction(decimal amount, bool isConfirmed)
        {
            var transaction = new Transaction()
            {
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                IsConfirmed = isConfirmed,
            };

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.CommitAsync();

            return transaction;
        }
    }
}
