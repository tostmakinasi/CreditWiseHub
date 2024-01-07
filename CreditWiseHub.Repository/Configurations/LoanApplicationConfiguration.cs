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
    public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
    {
        public void Configure(EntityTypeBuilder<LoanApplication> builder)
        {

            builder.HasKey(la => la.Id);

            builder.Property(la => la.RequestedAmount)
                .HasColumnType("decimal(18,2)"); 

            // Foreign key ilişkileri
            builder.HasOne(la => la.UserApp)
                .WithMany(la=> la.LoanApplications)
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.HasOne(la => la.LoanType)
                .WithMany()
                .HasForeignKey(la => la.LoanTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(la => la.Transaction)
                .WithMany()
                .HasForeignKey(la => la.TransactionId)
                .OnDelete(DeleteBehavior.SetNull); 
        }
    }
}
