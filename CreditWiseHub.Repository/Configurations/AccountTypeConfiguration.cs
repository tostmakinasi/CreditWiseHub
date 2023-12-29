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
    public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
    {
        public void Configure(EntityTypeBuilder<AccountType> builder)
        {
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Description).HasMaxLength(255);
            builder.Property(e => e.MinimumOpeningBalance).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.UtcNow);
        }
    }
}
