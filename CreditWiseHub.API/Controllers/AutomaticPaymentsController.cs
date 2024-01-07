using CreditWiseHub.Core.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutomaticPaymentsController : CustomBaseController
    {
        private readonly IAutomaticPaymentService _automaticPaymentService;

        public AutomaticPaymentsController(IAutomaticPaymentService automaticPaymentService)
        {
            _automaticPaymentService = automaticPaymentService;
        }

        [Authorize(Roles = "User, CashDesk")]
        [HttpDelete("{automaticPaymentId}")]
        public async Task<IActionResult> CancelAutomaticPaymentById(long automaticPaymentId) =>
            ActionResultInstance(await _automaticPaymentService.CancelRegisteredPaymentById(automaticPaymentId));

        [Authorize(Roles = "User, CashDesk")]
        [HttpGet("{automaticPaymentId}")]
        public async Task<IActionResult> GetRegisteredPaymentById(long automaticPaymentId) =>
            ActionResultInstance(await _automaticPaymentService.GetRegisteredPaymentById(automaticPaymentId));
    }
}
