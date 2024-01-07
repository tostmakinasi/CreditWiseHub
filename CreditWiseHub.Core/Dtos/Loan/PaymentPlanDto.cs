using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.Loan
{
    public class PaymentPlanDto
    {
        public int InstallmentNumber { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal PrincipalInInstallments { get; set; }
        public decimal InterestInInstallments { get; set; }
        public decimal RemainingPrincipalDebt { get; set; }

    }
}
