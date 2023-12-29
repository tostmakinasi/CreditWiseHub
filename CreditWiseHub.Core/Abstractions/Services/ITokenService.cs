using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface ITokenService
    {
        Task<TokenDto> CreateToken(UserApp user);
    }
}
