using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    /// <summary>
    /// An interface derived from a generic repository that manages customer tickets.
    /// </summary>
    /// <typeparam name="CustomerTicket">The type of customer ticket.</typeparam>
    /// <typeparam name="long">The type of ticket number.</typeparam>
    public interface ITicketRepository : IGenericRepository<CustomerTicket, long>
    {
        /// <summary>
        /// Returns a list of tickets by the given username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>A list of tickets.</returns>
        Task<List<CustomerTicket>> GetTicketsByUsername(string username);

        /// <summary>
        /// Returns a ticket by the given ticket number.
        /// </summary>
        /// <param name="ticketNumber">The ticket number.</param>
        /// <returns>A ticket.</returns>
        Task<CustomerTicket> GetTicketsById(long ticketNumber);
    }

}
