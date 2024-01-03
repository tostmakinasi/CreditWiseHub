namespace CreditWiseHub.Repository.Configurations
{
    using CreditWiseHub.Core.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserTransactionLimitConfiguration : IEntityTypeConfiguration<UserTransactionLimit>
    {
        public void Configure(EntityTypeBuilder<UserTransactionLimit> builder)
        {
            builder.ToTable("UserTransactionLimits");

            builder.HasKey(utl => utl.UserId);

            builder
                .HasOne(utl => utl.User)
                .WithOne(u => u.UserTransactionLimit)
                .HasForeignKey<UserTransactionLimit>(utl => utl.UserId);

            builder.Property(utl => utl.DailyTransactionLimit).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(utl => utl.DailyTransactionAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(utl => utl.InstantTransactionLimit).HasColumnType("decimal(18,2)").IsRequired();
        }
    }

}
