using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.AccountType
{
    public class AccountTypeDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }
        public int AccountsCount { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
