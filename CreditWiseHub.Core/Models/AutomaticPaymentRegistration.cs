using CreditWiseHub.Core.Commons;
using CreditWiseHub.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class AutomaticPaymentRegistration : BaseEntity<long>
    {
        public string Name { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }
        public int PaymentDueDay { get; set; }
        public int PaymentDueCount { get; set; }
        public int PaymentDuePaidCount { get; set; }
        public bool IsActive { get; set; }
        public UserApp User { get; set; }
        public string UserId { get; set; }
        public bool BelongToSystem { get; set; }

        public List<AutomaticPaymentHistory> AutomaticPaymentHistories { get; set; }
    }

    
}
