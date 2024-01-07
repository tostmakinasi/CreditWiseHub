using CreditWiseHub.Core.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TicketsController : CustomBaseController
    {
        private readonly ICustomerTicketService _ticketService;

        public TicketsController(ICustomerTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Authorize(Roles = "CustomerService, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOpenTickets() =>
            ActionResultInstance(await _ticketService.GetAllOpenTickets());

        [Authorize(Roles = "CustomerService, Admin")]
        [HttpPut("{ticketNumber}/upgradePriority")]
        public async Task<IActionResult> UpgradePriorityByTicketNumber(long ticketNumber) =>
           ActionResultInstance(await _ticketService.UpgradeTicketPriorityByTicketNumber(ticketNumber));

        [Authorize(Roles = "CustomerService, Admin")]
        [HttpPut("{ticketNumber}/inProgress")]
        public async Task<IActionResult> ChangeTicketInProgressByTicketNumber(long ticketNumber) =>
           ActionResultInstance(await _ticketService.PutTicketInProgress(ticketNumber));

        [Authorize(Roles = "CustomerService, Admin")]
        [HttpPut("{ticketNumber}/close")]
        public async Task<IActionResult> CloseTicketByTicketNumber(long ticketNumber) =>
           ActionResultInstance(await _ticketService.CloseTicketByTicketNumber(ticketNumber));
    }
}
