using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class Account : BaseEntity<Guid>
    {
        public string AccountNumber { get; set; }

        public string? Description { get; set; }

        [NotMapped]
        public decimal Balance { get; set; }

        public string UserAppId { get; set; }
        public UserApp UserApp { get; set; }

        public int AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }
    }
}
