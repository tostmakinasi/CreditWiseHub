using CreditWiseHub.Core.Commons;

namespace CreditWiseHub.Core.Models
{
    public class TransactionAffectedAccount : BaseEntity<long>
    {

        public string AccountNumber { get; set; }
        public string? Description { get; set; }
        public bool IsReceiverAccount { get; set; }

        public bool? IsExternal { get; set; }

        public decimal? BeforeBalance { get; set; }
        public decimal? AfterBalance { get; set; }

        public long TransactionId { get; set; }
        public Transaction Transaction { get; set; }

    }
}
