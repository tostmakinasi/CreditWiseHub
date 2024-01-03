using CreditWiseHub.Core.Commons;
using System.ComponentModel.DataAnnotations;

namespace CreditWiseHub.Core.Models
{
    public class UserTransactionLimit : IEntity
    {
        [Key]
        public string UserId { get; set; }
        public UserApp User { get; set; }

        public decimal DailyTransactionLimit { get; set; }
        public decimal DailyTransactionAmount { get; set; }

        public DateTime LastProcessDate { get; set; }

        public decimal InstantTransactionLimit { get; set; }
    }
}
