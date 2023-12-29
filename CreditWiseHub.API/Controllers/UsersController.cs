using CreditWiseHub.Core.Abstractions.Services;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto) => ActionResultInstance(await _userService.CreateAsync(createUserDto));

        [Authorize(Roles ="admin")]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetByUserName(string username) => ActionResultInstance(await _userService.GetByTCKNAsync(username));

        [Authorize(Roles = "admin")]
        [HttpPut("{username}/roles")]
        public async Task<IActionResult> AddRoleByUserName(string username, RoleDto roleDto) => ActionResultInstance(await _userService.AddRoleByTCKNAsync(username, roleDto));

        [Authorize(Roles = "user, admin")]
        [HttpPut("{username}")]
        public async Task<IActionResult> Update(string username, UpdateUserDto updateUserDto) => ActionResultInstance(await _userService.UpdateAsync(username, updateUserDto));

        [Authorize(Roles = "admin")]
        [HttpDelete("{username}")]
        public async Task<IActionResult> Remove(string username) => ActionResultInstance(await _userService.RemoveAsync(username));
    }
}
