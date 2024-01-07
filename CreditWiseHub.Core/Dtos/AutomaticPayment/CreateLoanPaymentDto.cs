using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.AutomaticPayment
{
    public class CreateLoanPaymentDto
    {
        public string Name { get; set; }
        public decimal PaymentAmount { get; set; }
        public int PaymentDueDay { get; set; }
        public int PaymentDueCount { get; set; }
    }
}
