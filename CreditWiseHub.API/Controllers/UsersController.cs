using CreditWiseHub.API.Filters;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CustomBaseController
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        public UsersController(IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto) => ActionResultInstance(await _userService.CreateAsync(createUserDto));

        [Authorize(Roles = "admin, auditor")]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetByUserName(string username) => ActionResultInstance(await _userService.GetByTCKNAsync(username));

        [Authorize(Roles = "admin")]
        [HttpPut("{username}/roles")]
        public async Task<IActionResult> AddRoleByUserName(string username, RoleDto roleDto) => ActionResultInstance(await _userService.AddRoleByTCKNAsync(username, roleDto));

        [Authorize(Roles = "User, Admin")]
        [AuthorizeByUsername]
        [HttpPut("{username}")]
        public async Task<IActionResult> Update(string username, UpdateUserDto updateUserDto) => ActionResultInstance(await _userService.UpdateAsync(username, updateUserDto));

        [Authorize(Roles = "admin")]
        [HttpDelete("{username}")]
        public async Task<IActionResult> Remove(string username) => ActionResultInstance(await _userService.RemoveAsync(username));

        [Authorize(Roles = "User, CashDesk")]
        [AuthorizeByUsername]
        [HttpPost("{username}/accounts")]
        public async Task<IActionResult> CreateAccountByUserName(string username, CreateAccountDto createAccountDto) => ActionResultInstance(await _accountService.CreateByUserName(username, createAccountDto));

    }
}
