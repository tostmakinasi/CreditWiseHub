using CreditWiseHub.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionType Type { get; set; }

        public bool Is { get; set; }

        public int? SenderAccountId { get; set; }

        public virtual Account Account { get; set; }


    }
}
