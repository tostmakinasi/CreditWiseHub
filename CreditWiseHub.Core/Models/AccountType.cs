using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class AccountType : BaseEntity<int>
    {
        public string Name { get; set; }
        public decimal MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
