using CreditWiseHub.API.Filters;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Dtos.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : CustomBaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByAccountNumber]
        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> GetAccountInfo(string accountNumber) =>
          ActionResultInstance(await _accountService.GetAccountInfoByAccountNumber(accountNumber));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByAccountNumber]
        [HttpPost("{accountNumber}/deposit")]
        public async Task<IActionResult> DepositAsync(string accountNumber, MoneyProcessAmountDto dto) =>
            ActionResultInstance(await _accountService.DepositMoneyByAccountNumber(accountNumber, dto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByAccountNumber]
        [HttpPost("{accountNumber}/withdraw")]
        public async Task<IActionResult> WithdrawAsync(string accountNumber, MoneyProcessAmountDto dto) =>
            ActionResultInstance(await _accountService.WithdrawMoneyByAccountNumber(accountNumber, dto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByAccountNumber]
        [HttpPost("{accountNumber}/[action]")]
        public async Task<IActionResult> InternalTransfer(string accountNumber, MoneyTransferDto dto) =>
            ActionResultInstance(await _accountService.InternalMoneyTransfer(accountNumber, dto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByAccountNumber]
        [HttpPost("{accountNumber}/[action]")]
        public async Task<IActionResult> ExternalTransfer(string accountNumber, MoneyExternalTransferDto dto) =>
            ActionResultInstance(await _accountService.ExternalTransfer(accountNumber, dto));

        [HttpGet("{accountNumber}/isExists")]
        public async Task<IActionResult> CheckInBankAccount(string accountNumber) =>
            ActionResultInstance(await _accountService.CheckAccountByAccountNumber(accountNumber));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByAccountNumber]
        [HttpGet("{accountNumber}/history")]
        public async Task<IActionResult> GetAccountHistoryByAccountNumber(string accountNumber) =>
            ActionResultInstance(await _accountService.GetAccountHistoryByAccountNumber(accountNumber));
    }
}
