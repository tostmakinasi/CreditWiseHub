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
    public interface ICustomerTicketService
    {
        Task<Response<List<TicketWithPriorityDto>>> GetAllOpenTickets();
        Task<Response<NoDataDto>> CloseTicketByTicketNumber(long ticketNumber);
        Task<Response<TicketWithPriorityDto>> UpgradeTicketPriorityByTicketNumber(long ticketNumber);
        Task<Response<TicketDto>> CreateTicketByUsername(string username, CreateTicketDto createTicketDto);
        Task<Response<List<TicketDto>>> GetTicketByUsername(string username);
        Task<Response<TicketWithPriorityDto>> PutTicketInProgress(long ticketNumber);
    }
}
