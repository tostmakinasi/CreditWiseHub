using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWiseHub.Repository.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.Property(t => t.IsConfirmed)
                .IsRequired();

            // Relationships
            builder.HasMany(t => t.AffectedAccounts)
                .WithOne(taa => taa.Transaction)
                .HasForeignKey(taa => taa.TransactionId);

        }
    }
}
