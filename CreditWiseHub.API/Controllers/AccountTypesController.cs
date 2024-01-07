using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Dtos.AccountType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountTypesController : CustomBaseController
    {
        private readonly IAccountTypeService _accountTypeService;

        public AccountTypesController(IAccountTypeService accountTypeService)
        {
            _accountTypeService = accountTypeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll() => ActionResultInstance(await _accountTypeService.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => ActionResultInstance(await _accountTypeService.GetById(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountTypeDto createAccountTypeDto) => ActionResultInstance(await _accountTypeService.CreateAsync(createAccountTypeDto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAccountTypeDto updateAccountTypeDto) => ActionResultInstance(await _accountTypeService.Update(id, updateAccountTypeDto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id) => ActionResultInstance(await _accountTypeService.Delete(id));
    }
}
