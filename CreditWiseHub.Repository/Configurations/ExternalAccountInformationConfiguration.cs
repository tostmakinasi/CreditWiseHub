using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWiseHub.Repository.Configurations
{
    public class ExternalAccountInformationConfiguration : IEntityTypeConfiguration<ExternalAccountInformation>
    {
        public void Configure(EntityTypeBuilder<ExternalAccountInformation> builder)
        {
            builder.HasKey(eai => eai.Id);

            builder.Property(eai => eai.AccountNumber)
                .IsRequired();

            builder.Property(eai => eai.BankName)
                .IsRequired();

            builder.Property(eai => eai.OwnerFullName)
                .IsRequired();

        }
    }
}
