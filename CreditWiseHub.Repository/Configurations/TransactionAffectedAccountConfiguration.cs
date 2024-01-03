using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWiseHub.Repository.Configurations
{
    public class TransactionAffectedAccountConfiguration : IEntityTypeConfiguration<TransactionAffectedAccount>
    {
        public void Configure(EntityTypeBuilder<TransactionAffectedAccount> builder)
        {
            builder.HasKey(taa => taa.Id);
            builder.Property(taa => taa.AccountNumber)
                .IsRequired();

            builder.Property(taa => taa.Description)
                .HasMaxLength(255);

            builder.Property(taa => taa.BeforeBalance)
                .HasColumnType("decimal(18, 2)");

            builder.Property(taa => taa.AfterBalance)
                .HasColumnType("decimal(18, 2)");

            // Relationships
            builder.HasOne(taa => taa.Transaction)
                .WithMany(t => t.AffectedAccounts)
                .HasForeignKey(taa => taa.TransactionId);


        }
    }
}
