using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWiseHub.Repository.Configurations
{
    public class UserAppConfiguration : IEntityTypeConfiguration<UserApp>
    {
        public void Configure(EntityTypeBuilder<UserApp> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Surname).IsRequired().HasMaxLength(50);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(11);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.UtcNow);
            builder.HasQueryFilter(x => x.IsActive == true);


        }
    }
}
