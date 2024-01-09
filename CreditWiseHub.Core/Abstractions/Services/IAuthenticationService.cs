using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Dtos.User;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// Interface for managing authentication-related operations.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Creates a new authentication token based on the provided login credentials.
        /// </summary>
        /// <param name="loginDto">The login credentials for creating the authentication token.</param>
        /// <returns>A response containing the authentication token details, or an error message if unsuccessful.</returns>
        Task<Response<TokenDto>> CreateToken(LoginDto loginDto);

        /// <summary>
        /// Creates a new authentication token using a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token for creating a new authentication token.</param>
        /// <returns>A response containing the new authentication token details, or an error message if unsuccessful.</returns>
        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        /// <summary>
        /// Revokes a refresh token, rendering it invalid for future authentication token requests.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <returns>A response indicating success or failure, along with an error message if applicable.</returns>
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);
    }

}
