using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Loan;
using CreditWiseHub.Core.Dtos.LoanApplication;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// Interface for managing loan application-related operations.
    /// </summary>
    public interface ILoanApplicationService
    {
        /// <summary>
        /// Retrieves a list of waiting loan applications.
        /// </summary>
        /// <returns>A response containing the list of waiting loan applications, or an error message if unsuccessful.</returns>
        Task<Response<List<LoanApplicationListDto>>> GetWaitingLoanApplicationsAsync();

        /// <summary>
        /// Approves a loan application based on the application number and approver ID.
        /// </summary>
        /// <param name="applicationNumber">The application number of the loan application to approve.</param>
        /// <param name="approverId">The ID of the user approving the loan application.</param>
        /// <returns>A response indicating success or failure, along with an error message if applicable.</returns>
        Task<Response<NoDataDto>> ApproveLoanApplication(long applicationNumber, string approverId);

        /// <summary>
        /// Rejects a loan application based on the application number and approver ID.
        /// </summary>
        /// <param name="applicationNumber">The application number of the loan application to reject.</param>
        /// <param name="approverId">The ID of the user rejecting the loan application.</param>
        /// <returns>A response indicating success or failure, along with an error message if applicable.</returns>
        Task<Response<NoDataDto>> RejectLoanApplication(long applicationNumber, string approverId);

        /// <summary>
        /// Retrieves the payment plan for a loan application based on the application number.
        /// </summary>
        /// <param name="applicationNumber">The application number of the loan application to retrieve the payment plan for.</param>
        /// <returns>A response containing the payment plan for the loan application, or an error message if unsuccessful.</returns>
        Task<Response<List<PaymentPlanDto>>> GetLoanPaymentPlanByApplicationNumber(long applicationNumber);

        /// <summary>
        /// Applies for a loan for a specific user based on the provided details.
        /// </summary>
        /// <param name="username">The username associated with the loan application.</param>
        /// <param name="createLoanApplicationDto">Details for creating the loan application.</param>
        /// <returns>A response containing the status of the loan application, or an error message if unsuccessful.</returns>
        Task<Response<LoanApplicationStatusDto>> ApplyLoanByUsername(string username, CreateLoanApplicationDto createLoanApplicationDto);

        /// <summary>
        /// Retrieves the status of a loan application based on the application number.
        /// </summary>
        /// <param name="applicationNumber">The application number of the loan application to retrieve the status for.</param>
        /// <returns>A response containing the status of the loan application, or an error message if unsuccessful.</returns>
        Task<Response<LoanApplicationStatusDto>> GetApplicationStatusByApplicationNumber(long applicationNumber);
    }

}
