using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    /// <summary>
    /// Interface for managing loan applications in the repository.
    /// </summary>
    public interface ILoanApplicationRepository : IGenericRepository<LoanApplication, long>
    {
        /// <summary>
        /// Retrieves a loan application with its associated loan type based on the application number.
        /// </summary>
        /// <param name="applicationNumber">The application number to retrieve the loan application for.</param>
        /// <returns>The loan application with its associated loan type, or null if not found.</returns>
        Task<LoanApplication> GetWithLoanTypeByApplicationNumber(long applicationNumber);
    }

}
