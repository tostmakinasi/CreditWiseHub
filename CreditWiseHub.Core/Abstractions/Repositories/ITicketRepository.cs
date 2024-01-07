using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    public interface ITicketRepository : IGenericRepository<CustomerTicket,long>
    {
        Task<List<CustomerTicket>> GetTicketsByUsername(string username);
        Task<CustomerTicket> GetTicketsById(long ticketNumber);
    }
}
