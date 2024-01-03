using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto) => ActionResultInstance(await _authenticationService.CreateToken(loginDto));

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto) => ActionResultInstance(await _authenticationService.RevokeRefreshToken(refreshTokenDto.Token));

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto) => ActionResultInstance(await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.Token));

    }
}
