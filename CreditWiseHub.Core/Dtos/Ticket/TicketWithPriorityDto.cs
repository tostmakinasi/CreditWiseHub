using CreditWiseHub.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos.Ticket
{
    public class TicketWithPriorityDto
    {
        public long TicketNumber { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
    }
}
