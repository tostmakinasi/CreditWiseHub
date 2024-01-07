using CreditWiseHub.Core.Dtos.LoanType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.LoanApplication
{
    public class LoanApplicationListDto
    {
        public long ApplicationNumber { get; set; }
        public int UserCreditScore { get; set; }
        public decimal RequestedAmount { get; set; }
        public int InstallmentCount { get; set; }
        public DateTime ApplicationDate { get; set; }
        public LoanTypeDto LoanType { get; set; }
        
    }
}
