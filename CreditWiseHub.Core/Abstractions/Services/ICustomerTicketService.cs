using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Ticket;
using CreditWiseHub.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// An interface that defines a service for managing customer tickets.
    /// </summary>
    public interface ICustomerTicketService
    {
        /// <summary>
        /// Returns a list of all open tickets with their priority information.
        /// </summary>
        /// <returns>A list of tickets with priority.</returns>
        Task<Response<List<TicketWithPriorityDto>>> GetAllOpenTickets();

        /// <summary>
        /// Closes the ticket by the given ticket number and returns the result.
        /// </summary>
        /// <param name="ticketNumber">The ticket number.</param>
        /// <returns>The result.</returns>
        Task<Response<NoDataDto>> CloseTicketByTicketNumber(long ticketNumber);

        /// <summary>
        /// Upgrades the priority of the ticket by the given ticket number and returns the ticket's latest status.
        /// </summary>
        /// <param name="ticketNumber">The ticket number.</param>
        /// <returns>The ticket's latest status.</returns>
        Task<Response<TicketWithPriorityDto>> UpgradeTicketPriorityByTicketNumber(long ticketNumber);

        /// <summary>
        /// Creates a new ticket with the given username and ticket creation information and returns the ticket information.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="createTicketDto">The ticket creation information.</param>
        /// <returns>The ticket information.</returns>
        Task<Response<TicketDto>> CreateTicketByUsername(string username, CreateTicketDto createTicketDto);

        /// <summary>
        /// Returns a list of tickets by the given username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>A list of tickets.</returns>
        Task<Response<List<TicketDto>>> GetTicketByUsername(string username);

        /// <summary>
        /// Puts the ticket in progress by the given ticket number and returns the ticket's latest status.
        /// </summary>
        /// <param name="ticketNumber">The ticket number.</param>
        /// <returns>The ticket's latest status.</returns>
        Task<Response<TicketWithPriorityDto>> PutTicketInProgress(long ticketNumber);
    }

}
