using CreditWiseHub.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CreditWiseHub.Repository.Contexts
{
    public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionAffectedAccount> AffectedAccounts { get; set; }
        public DbSet<UserTransactionLimit> UserTransactionLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);



            base.OnModelCreating(builder);
        }
    }
}
