using CreditWiseHub.Core.Commons;

namespace CreditWiseHub.Core.Models
{
    public class AccountType : BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public decimal MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
