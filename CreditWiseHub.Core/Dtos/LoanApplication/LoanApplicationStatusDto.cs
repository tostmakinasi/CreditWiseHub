using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.LoanApplication
{
    public class LoanApplicationStatusDto
    {
        public long ApplicationNumber { get; set; }
        public string Status { get; set; }
        public string? ApprovedDate { get; set; }
    }
}
