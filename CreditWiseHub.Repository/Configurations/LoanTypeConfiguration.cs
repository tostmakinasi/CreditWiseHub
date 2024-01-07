using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Repository.Configurations
{
    internal class LoanTypeConfiguration : IEntityTypeConfiguration<LoanType>
    {
        public void Configure(EntityTypeBuilder<LoanType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(lt => lt.Name)
           .IsRequired()
           .HasMaxLength(255);

            builder.Property(lt => lt.InterestRate)
                .HasColumnType("decimal(18,2)");

            builder.Property(lt => lt.MinLoanAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(lt => lt.MaxLoanAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasData(
                new LoanType
                {
                    Id=1,
                    Name = "İhtiyaç Kredisi",
                    InterestRate = 4,
                    MaxCreditScore = 500,
                    MinCreditScore = 100,
                    MaxInstallmentOption = 12,
                    MinInstallmentOption = 4,
                },
                new LoanType
                {
                    Id=2,
                    Name = "Ev Kredisi",
                    InterestRate = 10,
                    MaxCreditScore = 1000,
                    MinCreditScore = 600,
                    MaxInstallmentOption = 36,
                    MinInstallmentOption = 4,
                });
        }
    }
}
