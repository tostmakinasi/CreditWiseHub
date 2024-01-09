using CreditWiseHub.Core.Commons;
using CreditWiseHub.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Models
{
    public class CustomerTicket : BaseAuditableEntity<long>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }

        [ForeignKey(nameof(UserApp))]
        public string UserId { get; set; }
        public UserApp UserApp { get; set; }
    }
}
