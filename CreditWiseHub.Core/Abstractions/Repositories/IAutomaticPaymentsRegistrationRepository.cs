using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    /// <summary>
    /// Interface for managing automatic payment registrations in the repository.
    /// </summary>
    public interface IAutomaticPaymentsRegistrationRepository : IGenericRepository<AutomaticPaymentRegistration, long>
    {
        /// <summary>
        /// Retrieves a list of automatic payment registrations that are due for payment today based on specified due days.
        /// </summary>
        /// <param name="days">The list of days to consider for due payments.</param>
        /// <returns>A list of automatic payment registrations due for payment today.</returns>
        Task<List<AutomaticPaymentRegistration>> GetPaymentsDueToday(List<int> days);

        /// <summary>
        /// Retrieves a list of automatic payment registrations with their histories for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to retrieve automatic payment registrations for.</param>
        /// <returns>A list of automatic payment registrations with their histories for the specified user.</returns>
        Task<List<AutomaticPaymentRegistration>> GetPaymentsWithHistoriesByUserId(string userId);
    }

}
