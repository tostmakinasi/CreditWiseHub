using CreditWiseHub.Core.Commons;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace CreditWiseHub.Core.Models
{
    public class UserApp : IdentityUser, IEntity, IEntityWithId<string>, IAuditableEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual List<Account> Accounts { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public UserTransactionLimit UserTransactionLimit { get; set; }
        public virtual List<LoanApplication> LoanApplications { get; set; }
        public virtual List<AutomaticPaymentRegistration> AutomaticPaymentRegistrations { get; set; }
    }
}
