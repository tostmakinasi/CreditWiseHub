using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    public interface ILoanApplicationRepository : IGenericRepository<LoanApplication,long>
    {
        Task<LoanApplication> GetWithLoanTypeByApplicationNumber(long applicationNumber);
    }
}
