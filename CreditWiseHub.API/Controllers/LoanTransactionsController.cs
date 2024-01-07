using CreditWiseHub.Core.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoanTransactionsController : CustomBaseController
    {
        private readonly ILoanApplicationService _loanApplicationService;

        public LoanTransactionsController(ILoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [Authorize(Roles = "Auditor")]
        [HttpGet]
        public async Task<IActionResult> GetWaitingApplications() =>
            ActionResultInstance(await _loanApplicationService.GetWaitingLoanApplicationsAsync());

        [Authorize(Roles = "User, CashDesk, Auditor")]
        [HttpGet("{applicationNumber}")]
        public async Task<IActionResult> GetLoanPaymentPlan(long applicationNumber) =>
            ActionResultInstance(await _loanApplicationService.GetLoanPaymentPlanByApplicationNumber(applicationNumber));

        [Authorize(Roles = "Auditor")]
        [HttpPost("{applicationNumber}/approve")]
        public async Task<IActionResult> ApproveApplication(long applicationNumber) =>
          ActionResultInstance(await _loanApplicationService.ApproveLoanApplication(applicationNumber, GetAutUserId()));

        [Authorize(Roles = "Auditor")]
        [HttpPost("{applicationNumber}/reject")]
        public async Task<IActionResult> RejectApplication(long applicationNumber) =>
          ActionResultInstance(await _loanApplicationService.RejectLoanApplication(applicationNumber, GetAutUserId()));
    }
}
