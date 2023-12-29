using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.AccountType
{
    public class UpdateAccountTypeDto
    {
        public string? Name { get; set; }
        public decimal? MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }
    }
}
