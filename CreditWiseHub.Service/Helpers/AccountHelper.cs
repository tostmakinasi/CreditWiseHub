using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Helpers
{
    public class AccountHelper
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AccountHelper(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public bool CheckOpeningBalance(decimal openingBalance, decimal minimumOpeningBalance)
        {
            if (openingBalance >= minimumOpeningBalance)
                return true;

            return false;
        }
        public async Task<string> GenerateAccountNumberAsync()
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

        public string GenerateUserNameWithCensor(string userName)
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

        public async Task<(bool result, string error)> CheckUserLimitationFotWithdraw(decimal amount, Account account, UserTransactionLimit userLimitation)
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

        public async Task<Account> GetValidAccountByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumber(accountNumber);
            if (account is null)
                throw new NotFoundException($"Account ({accountNumber}) not found");
            return account;
        }

      
    }
}
