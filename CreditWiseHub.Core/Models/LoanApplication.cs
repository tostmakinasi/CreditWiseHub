using CreditWiseHub.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class LoanApplication : BaseAuditableEntity<long>
    {
        public string UserId { get; set; }
        public UserApp UserApp { get; set; }
        public int LoanTypeId { get; set; }
        public LoanType LoanType { get; set; }
        public decimal RequestedAmount { get; set; }
        public int InstallmentCount { get; set; }
        public bool? IsRejected { get; set; }
        public bool IsApproved { get; set; }
        public string? ApproverId { get; set; } 
        public DateTime? ApprovalDate { get; set; } 

        public long? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
