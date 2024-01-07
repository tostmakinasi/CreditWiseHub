using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Loan;
using CreditWiseHub.Core.Dtos.LoanApplication;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface ILoanApplicationService
    {
        Task<Response<List<LoanApplicationListDto>>> GetWaitingLoanApplicationsAsync();
        Task<Response<NoDataDto>> ApproveLoanApplication(long applicationNumber, string ApproverId);
        Task<Response<NoDataDto>> RejectLoanApplication(long applicationNumber, string ApproverId);
        Task<Response<List<PaymentPlanDto>>> GetLoanPaymentPlanByApplicationNumber(long applicationNumber);
        Task<Response<LoanApplicationStatusDto>> ApplyLoanByUsername(string username, CreateLoanApplicationDto createLoanApplicationDto);
        Task<Response<LoanApplicationStatusDto>> GetApplicationStatusByApplicationNumber(long applicationNumber);
    }
}
