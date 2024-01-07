using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.AutomaticPayment
{
    public class AutomaticPaymentDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }
        public int PaymentDueDay { get; set; }
        public int PaymentDueCount { get; set; }

        public List<PaymentHistoryDto> AutomaticPaymentHistories { get; set; }
    }

   
}
