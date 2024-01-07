using CreditWiseHub.Core.Commons;
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
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<AutomaticPaymentRegistration> AutomaticPaymentRegistrations { get; set; }
        public DbSet<AutomaticPaymentHistory> AutomaticPaymentHistories { get; set; }
        public DbSet<ExternalAccountInformation> ExternalAccountInformations { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionAffectedAccount> AffectedAccounts { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<UserTransactionLimit> UserTransactionLimits { get; set; }
        public DbSet<CustomerTicket> Tickets { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IAuditableEntity entityReference)
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entityReference.IsActive = false;
                            entityReference.UpdatedDate = DateTime.UtcNow;
                            Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
                            break;
                        case EntityState.Modified:
                            entityReference.UpdatedDate = DateTime.UtcNow;
                            Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
                            break;
                        case EntityState.Added:
                            entityReference.CreatedDate = DateTime.UtcNow;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
