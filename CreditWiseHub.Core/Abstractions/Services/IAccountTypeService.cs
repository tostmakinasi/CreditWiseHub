using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AccountType;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IAccountTypeService
    {
        Task<Response<List<AccountTypeDto>>> GetAll();
        Task<Response<AccountTypeDetailDto>> GetById(int id);
        Task<Response<AccountTypeDetailDto>> CreateAsync(CreateAccountTypeDto accountTypeDto);
        Task<Response<NoDataDto>> Update(int id, UpdateAccountTypeDto accountTypeDto);
        Task<Response<NoDataDto>> Delete(int id);
    }
}
