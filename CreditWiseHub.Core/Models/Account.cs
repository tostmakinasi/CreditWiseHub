using CreditWiseHub.Core.Commons;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditWiseHub.Core.Models
{
    public class Account : BaseAuditableEntity<Guid>
    {
        public string AccountNumber { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }

        public decimal Balance { get; set; }

        public string UserAppId { get; set; }
        public UserApp UserApp { get; set; }

        public int AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }

        [NotMapped]
        public List<TransactionAffectedAccount> TransactionAffectedAccounts { get; set; }

    }
}
