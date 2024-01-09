using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWiseHub.Repository.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.AccountNumber).IsRequired();
            builder.Property(e => e.Description);
            builder.Property(e => e.UserAppId).IsRequired();
            builder.HasOne(e => e.UserApp)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.UserAppId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.AccountTypeId).IsRequired();
            builder.HasOne(e => e.AccountType)
                .WithMany(e => e.Accounts)
                .HasForeignKey(e => e.AccountTypeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.UtcNow);
            builder.HasQueryFilter(x => x.IsActive);
            builder.Property<uint>("Version")
              .IsRowVersion();

        }
    }
}
