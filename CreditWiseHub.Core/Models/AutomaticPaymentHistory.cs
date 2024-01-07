using CreditWiseHub.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class AutomaticPaymentHistory : BaseEntity<long>
    {
        public long RegistrationId { get; set; }
        public AutomaticPaymentRegistration Registration { get; set; }
        public string Comment { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPaid { get; set; }

        public long? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
