using CreditWiseHub.Core.Dtos.LoanType;
using CreditWiseHub.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface ILoanTypeService
    {
        Task<Response<LoanTypeDto>> GetLoanTypeByIdAsync(int loanTypeId);
        Task<Response<List<LoanTypeDto>>> GetAllLoanTypesAsync();
        Task<Response<LoanTypeDto>> AddLoanTypeAsync(CreateLoanTypeDto createLoanTypeDto);
        Task<Response<LoanTypeDto>> UpdateLoanTypeAsync(int loanTypeId, UpdateLoanTypeDto updateLoanTypeDto);
        Task<Response<NoDataDto>> DeleteLoanTypeAsync(int loanTypeId);
    }
}
