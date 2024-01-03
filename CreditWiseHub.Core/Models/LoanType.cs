﻿using CreditWiseHub.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class LoanType:BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public int MaxInstallmentOption { get; set; }
        public int MinInstallmentOption { get; set; }
        public decimal MinLoanAmount { get; set; }
        public decimal MaxLoanAmount { get; set; }
        public int MinCreditScore { get; set; }
        public int MaxCreditScore { get; set; }
        
    }
}
