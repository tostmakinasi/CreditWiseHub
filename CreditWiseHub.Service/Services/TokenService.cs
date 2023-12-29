using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CreditWiseHub.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;

        private readonly CustomTokenOption _tokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }

        private async Task<IEnumerable<Claim>> GetClaims(UserApp user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

            };

            userList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));

            return userList;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new byte[32];

            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }

        public async Task<TokenDto> CreateToken(UserApp user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                claims: await GetClaims(user),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDto()
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;
        }
    }
}
