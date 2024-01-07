using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.AutomaticPayment
{
    public class PaymentProcessResultDto
    {
        public DateTime PaymentDate { get; set; }
        public long TransactionId { get; set; }
        public string Comment { get; set; }
    }
}
