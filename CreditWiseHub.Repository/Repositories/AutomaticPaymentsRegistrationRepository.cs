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
    public class AutomaticPaymentsRegistrationRepository : GenericRepository<AutomaticPaymentRegistration, long>, IAutomaticPaymentsRegistrationRepository
    {
        public AutomaticPaymentsRegistrationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<AutomaticPaymentRegistration>> GetPaymentsDueToday(List<int> days)
        {
            var payments = await _dbSet.Where(x => x.IsActive && days.Contains(x.PaymentDueDay)).ToListAsync();

            return payments;
        }

        
    }
}
