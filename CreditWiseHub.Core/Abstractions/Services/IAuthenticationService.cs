using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Dtos.User;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateToken(LoginDto loginDto);
        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);
    }
}
