using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.LoanApplication
{
    public class CreateLoanApplicationDto
    {
        public int LoanTypeId { get; set; }
        public decimal RequestedAmount { get; set; }
        public int InstallmentCount { get; set; }
    }
}
