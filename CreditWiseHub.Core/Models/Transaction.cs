using CreditWiseHub.Core.Commons;

namespace CreditWiseHub.Core.Models
{
    public class Transaction : BaseEntity<long>
    {

        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsConfirmed { get; set; }
        public List<TransactionAffectedAccount> AffectedAccounts { get; set; }
    }
}
