using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    public interface IAutomaticPaymentsRegistrationRepository : IGenericRepository<AutomaticPaymentRegistration,long>
    {
        Task<List<AutomaticPaymentRegistration>> GetPaymentsDueToday(List<int> days);
        Task<List<AutomaticPaymentRegistration>> GetPaymentsWithHistoriesByUserId(string userId);
    }
}
