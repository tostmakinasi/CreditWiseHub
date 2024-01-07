using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.User;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using UserUserNameValidation;

namespace CreditWiseHub.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGenericRepository<UserTransactionLimit, string> _userTransactionLimitRepository;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(UserManager<UserApp> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IGenericRepository<UserTransactionLimit, string> userTransactionLimitRepository, IUnitOfWork unitOfWork, IAccountService accountService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _userTransactionLimitRepository = userTransactionLimitRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }

        public async Task<Response<NoDataDto>> AddRoleByTCKNAsync(string userName, RoleDto roleDto)
        {

            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<NoDataDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var isExistsRole = await _roleManager.RoleExistsAsync(roleDto.RoleName);

            if (!isExistsRole)
                return Response<NoDataDto>.Fail("Role not found", HttpStatusCode.NotFound, true);

            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                if (!removeRolesResult.Succeeded)
                {
                    var removeErrors = removeRolesResult.Errors.Select(x => x.Description).ToList();
                    return Response<NoDataDto>.Fail(new ErrorDto(removeErrors, false), HttpStatusCode.InternalServerError);
                }
            }

            var result = await _userManager.AddToRoleAsync(user, roleDto.RoleName);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return Response<NoDataDto>.Fail(new ErrorDto(errors, false), HttpStatusCode.InternalServerError);

            }

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        public async Task<Response<UserDto>> CreateAsync(CreateUserDto createUserDto)
        {
            //var tcknCheck = await UserNameValidationService(createUserDto);

            //if(!tcknCheck)
            //    return Response<UserDto>.Fail("User's TCKN does not match.", HttpStatusCode.BadRequest,true);

            var user = _mapper.Map<UserApp>(createUserDto);

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return Response<UserDto>.Fail(new ErrorDto(errors, true), HttpStatusCode.BadRequest);

            }
            await _userManager.AddToRoleAsync(user, RoleNames.User.ToString());

            var userLimits = new UserTransactionLimit
            {
                UserId = user.Id,
                User = user,
                InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                DailyTransactionAmount = 0,
                LastProcessDate = DateTime.UtcNow,
            };

            await _userTransactionLimitRepository.AddAsync(userLimits);
            await _unitOfWork.CommitAsync();

            var defaultAccount = new CreateAccountDto {
                Name = "Vadesiz Hesap",
                OpeningBalance = 0,
                AccountTypeId = 1,
                Description = ""
            };
            var accountInfo = await _accountService.CreateByUserName(user.UserName, defaultAccount);


            var userDto = _mapper.Map<UserDto>(user);
            userDto.DefaultAccountNumber = accountInfo.Data.AccountNumber;

            return Response<UserDto>.Success(userDto, HttpStatusCode.Created);
        }

        public async Task<Response<UserDto>> GetByTCKNAsync(string userName)
        {
            var user = await _userManager.Users.Include(x=> x.Accounts).Where(x=> x.UserName == userName).FirstOrDefaultAsync();

            if (user is null)
                return Response<UserDto>.Fail("User not found", HttpStatusCode.NotFound, true);
            
            return Response<UserDto>.Success(_mapper.Map<UserDto>(user), HttpStatusCode.OK);
        }

        private static async Task<bool> UserNameValidationService(CreateUserDto userDto)
        {
            long userTckn;
            if (!long.TryParse(userDto.TCKN, out userTckn))
                return false;

            using var client = new KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);
            var response = await client.TCKimlikNoDogrulaAsync(userTckn, userDto.Name, userDto.Surname, userDto.DateOfBirth.Year);

            return response.Body.TCKimlikNoDogrulaResult;
        }

        public async Task<Response<NoDataDto>> UpdateAsync(string userName, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<NoDataDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            user.Email = updateUserDto.Email;

            await _userManager.UpdateAsync(user);

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        public async Task<Response<NoDataDto>> RemoveAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return Response<NoDataDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return Response<NoDataDto>.Fail(new ErrorDto(errors, true), HttpStatusCode.BadRequest);

            }

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }
    }
}
