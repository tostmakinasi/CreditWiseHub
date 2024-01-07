using CreditWiseHub.API.Filters;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.LoanApplication;
using CreditWiseHub.Core.Dtos.Ticket;
using CreditWiseHub.Core.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : CustomBaseController
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly IAutomaticPaymentService _automaticPaymentService;
        private readonly ICustomerTicketService _ticketService;
        public UsersController(IUserService userService, IAccountService accountService, IAutomaticPaymentService automaticPaymentService, ILoanApplicationService loanApplicationService, ICustomerTicketService ticketService)
        {
            _userService = userService;
            _accountService = accountService;
            _automaticPaymentService = automaticPaymentService;
            _loanApplicationService = loanApplicationService;
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto) => 
            ActionResultInstance(await _userService.CreateAsync(createUserDto));

        [Authorize(Roles = "User, Admin, Auditor")]
        [AuthorizeByUsername]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetByUserName(string username) => 
            ActionResultInstance(await _userService.GetByTCKNAsync(username));

        [Authorize(Roles = "User, CustomerService")]
        [AuthorizeByUsername]
        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateByUsername(string username, UpdateUserDto updateUserDto) =>
            ActionResultInstance(await _userService.UpdateAsync(username, updateUserDto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{username}")]
        public async Task<IActionResult> RemoveByUsername(string username) =>
            ActionResultInstance(await _userService.RemoveAsync(username));

        [Authorize(Roles = "Admin")]
        [HttpPut("{username}/roles")]
        public async Task<IActionResult> AddRoleByUsername(string username, RoleDto roleDto) => 
            ActionResultInstance(await _userService.AddRoleByTCKNAsync(username, roleDto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByUsername]
        [HttpGet("{username}/accounts")]
        public async Task<IActionResult> GetAccountsByUserName(string username) =>
            ActionResultInstance(await _accountService.GetUserAccountsByUsername(username));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByUsername]
        [HttpPost("{username}/accounts")]
        public async Task<IActionResult> CreateAccountByUserName(string username, CreateAccountDto createAccountDto) => 
            ActionResultInstance(await _accountService.CreateByUserName(username, createAccountDto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByUsername]
        [HttpPost("{username}/loanTransactions")]
        public async Task<IActionResult> ApplyLoanByUserName(string username, CreateLoanApplicationDto createLoanApplicationDto) => 
            ActionResultInstance(await _loanApplicationService.ApplyLoanByUsername(username, createLoanApplicationDto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByUsername]
        [HttpPost("{username}/automaticPayments")]
        public async Task<IActionResult> AddAutomaticPaymentInvoice(string username, CreateInvoiceAutomaticPaymentDto paymentDto) => 
            ActionResultInstance(await _automaticPaymentService.RegistrationInvoiceAutomaticPayment(username, paymentDto));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByUsername]
        [HttpGet("{username}/automaticPayments")]
        public async Task<IActionResult> GetAutomaticPaymentsByUserName(string username) => 
            ActionResultInstance(await _automaticPaymentService.GetRegisteredPaymentsByUserName(username));

        [Authorize(Roles = "User")]
        [AuthorizeByUsername]
        [HttpPost("{username}/tickets")]
        public async Task<IActionResult> CreateTicketByUsername(string username,CreateTicketDto createTicketDto) =>
           ActionResultInstance(await _ticketService.CreateTicketByUsername(username, createTicketDto));

        [Authorize(Roles = "User,CustomerService")]
        [AuthorizeByUsername]
        [HttpGet("{username}/tickets")]
        public async Task<IActionResult> GetTicketsByUsername(string username) =>
           ActionResultInstance(await _ticketService.GetTicketByUsername(username));
    }
}
