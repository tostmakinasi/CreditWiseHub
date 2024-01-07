using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Dtos.AccountType;
using CreditWiseHub.Core.Dtos.LoanType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CreditWiseHub.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoanTypesController : CustomBaseController
    {
        private readonly ILoanTypeService _loanTypeService;

        public LoanTypesController(ILoanTypeService loanTypeService)
        {
            _loanTypeService = loanTypeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll() => ActionResultInstance(await _loanTypeService.GetAllLoanTypesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => ActionResultInstance(await _loanTypeService.GetLoanTypeByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateLoanTypeDto createLoanTypeDto) => ActionResultInstance(await _loanTypeService.AddLoanTypeAsync(createLoanTypeDto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLoanTypeDto updateLoanTypeDto) => ActionResultInstance(await _loanTypeService.UpdateLoanTypeAsync(id, updateLoanTypeDto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id) => ActionResultInstance(await _loanTypeService.DeleteLoanTypeAsync(id));
    }
}
