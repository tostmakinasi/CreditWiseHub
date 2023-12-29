using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Dtos.User;
using CreditWiseHub.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CreditWiseHub.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenRepository;

        public AuthenticationService(ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenRepository)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenRepository = userRefreshTokenRepository;
        }

        public async Task<Response<TokenDto>> CreateToken(LoginDto loginDto)
        {
            if (loginDto is null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByNameAsync(loginDto.TCKN);

            if (user is null)
                return Response<TokenDto>.Fail("Username or Password is wrong", HttpStatusCode.BadRequest, true);

            if (!(await _userManager.CheckPasswordAsync(user, loginDto.Password)))
                return Response<TokenDto>.Fail("Username or Password is wrong", HttpStatusCode.BadRequest, true);

            var tokenDto = await _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenRepository.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken is null)
            {
                await _userRefreshTokenRepository.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = tokenDto.RefreshToken,
                    Expiration = tokenDto.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = tokenDto.RefreshToken;
                userRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, HttpStatusCode.OK);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenRepository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken is null)
                return Response<TokenDto>.Fail("Refresh token not found", HttpStatusCode.NotFound, true);

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if (user is null)
                return Response<TokenDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var tokenDto = await _tokenService.CreateToken(user);

            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, HttpStatusCode.OK);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenRepository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken is null)
                return Response<NoDataDto>.Fail("Refresh token not found", HttpStatusCode.NotFound, true);

            _userRefreshTokenRepository.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }
    }
}
