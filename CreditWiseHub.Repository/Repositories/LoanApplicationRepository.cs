using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Repository.Repositories
{
    public class LoanApplicationRepository : GenericRepository<LoanApplication, long>, ILoanApplicationRepository
    {
        public LoanApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<LoanApplication> GetWithLoanTypeByApplicationNumber(long applicationNumber)
        {
            var loanApplication = await _dbSet.Include(x=> x.LoanType).FirstOrDefaultAsync(x=> x.Id == applicationNumber);
            return loanApplication;
        }
    }
}
