using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Dtos.User;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IUserService
    {
        Task<Response<UserDto>> CreateAsync(CreateUserDto createUserDto);
        Task<Response<UserDto>> GetByTCKNAsync(string userName);
        Task<Response<NoDataDto>> AddRoleByTCKNAsync(string userName, RoleDto roleDto);
        Task<Response<NoDataDto>> UpdateAsync(string userName, UpdateUserDto updateUserDto);
        Task<Response<NoDataDto>> RemoveAsync(string userName);
    }
}
