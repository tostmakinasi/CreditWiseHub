using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Repository.Configurations
{
    public class AutomaticPaymentRegistrationConfiguration : IEntityTypeConfiguration<AutomaticPaymentRegistration>
    {
        public void Configure(EntityTypeBuilder<AutomaticPaymentRegistration> builder)
        {
            builder.Property(apr => apr.PaymentType)
                .IsRequired();

            builder.Property(apr => apr.PaymentAmount)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(apr => apr.PaymentDueDay)
                .IsRequired();

            builder.Property(apr => apr.PaymentDueCount)
                .IsRequired();

            builder.Property(apr => apr.IsActive)
                .IsRequired();

            builder.Property(apr => apr.UserId)
                .IsRequired();

            builder.Property(apr => apr.BelongToSystem)
                .IsRequired();

            builder.HasOne(e => e.User)
                .WithMany(u => u.AutomaticPaymentRegistrations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
