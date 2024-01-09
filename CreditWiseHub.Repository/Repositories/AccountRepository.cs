using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Commons;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CreditWiseHub.Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account, Guid>, IAccountRepository
    {
        private readonly IGenericRepository<ExternalAccountInformation, long> _externalAccountInformationRepository;
        public AccountRepository(AppDbContext context, IGenericRepository<ExternalAccountInformation, long> externalAccountInformationRepository) : base(context)
        {
            _externalAccountInformationRepository = externalAccountInformationRepository;
        }

        public async Task AddExternalAccount(ExternalAccountInformation externalAccountInformation)
        {
            await _externalAccountInformationRepository.AddAsync(externalAccountInformation);
        }

        public async Task<decimal> GetAccountBalanceByAccountNumber(string accountNumber)
        {
            var balance = await _dbSet.Where(x => x.AccountNumber == accountNumber).Select(x => x.Balance).FirstOrDefaultAsync();
            return balance;
        }

        public async Task<Account?> GetAccountByAccountNumber(string accountNumber)
        {
            var account = await _dbSet.Where(x => x.AccountNumber == accountNumber).Include(x=> x.AccountType).Include(x=> x.UserApp).FirstOrDefaultAsync();
            return account;
        }

        public async Task<Account> GetAccountByAccountNumberWithUserAndUserLimits(string accountNumber)
        {
            var account = await _dbSet.Where(x => x.AccountNumber == accountNumber)
                .Include(x => x.AccountType)
                .Include(x => x.UserApp).ThenInclude(x=> x.UserTransactionLimit)
                .FirstOrDefaultAsync();
            return account;
        }

        public async Task<string> GetAccountHolderFullNameByAccountNumber(string accountNumber)
        {
            var fullName = await _dbSet.Where(x => x.AccountNumber == accountNumber).Include(x => x.UserApp).Select(x => $"{x.UserApp.Name} {x.UserApp.Surname}").FirstOrDefaultAsync();

            return fullName;
        }

        public async Task<List<Account>> GetAccountsByUsername(string username)
        {
            var accounts = await _dbSet.Include(x => x.UserApp).Where(x => x.UserApp.UserName == username).Include(x => x.AccountType).ToListAsync();
            return accounts;
        }

        public async Task<Account> GetAccountsWithTransactionsByAccountNumber(string accountNumber)
        {
            var account = await _dbSet.Include(x => x.UserApp)
                .Where(x => x.AccountNumber == accountNumber).FirstOrDefaultAsync();

            var accountsTransactions = await _context.AffectedAccounts.Include(x => x.Transaction).Where(x => x.AccountNumber == accountNumber && (x.IsExternal == null || x.IsExternal! == false)).ToListAsync();

            account.TransactionAffectedAccounts = accountsTransactions;

            return account;
        }

        public async Task<Account> GetUserDefaultAccountByUserIdWithUserAndUserLimits(string userID)
        {
            var account = await _dbSet.Where(x => x.UserAppId == userID && x.AccountTypeId == 1)
                .Include(x => x.AccountType)
                .Include(x => x.UserApp).ThenInclude(x => x.UserTransactionLimit)
                .FirstOrDefaultAsync();
            return account;
        }

        public Task<bool> UserHaveDefaultAccountAnyAsync(Expression<Func<Account, bool>> expression)
        {
            var result = _dbSet.Where(expression).AnyAsync();

            return result;
        }
    }
}
