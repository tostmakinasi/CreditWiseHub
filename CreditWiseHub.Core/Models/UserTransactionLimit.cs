using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class UserTransactionLimit
    {
        [Key]
        public string UserId { get; set; }
        public UserApp User { get; set; }

        public decimal DailyTransactionAmount { get; set; }
    }
}
