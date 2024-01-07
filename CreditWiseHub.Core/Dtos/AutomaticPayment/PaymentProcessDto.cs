using CreditWiseHub.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.AutomaticPayment
{
    public class PaymentProcessDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Description { get; set; }

    }
}
