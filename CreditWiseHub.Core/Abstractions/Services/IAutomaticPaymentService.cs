using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// Interface for managing automatic payment-related operations.
    /// </summary>
    public interface IAutomaticPaymentService
    {
        /// <summary>
        /// Registers an automatic payment for a loan based on the provided details.
        /// </summary>
        /// <param name="userId">The user ID associated with the automatic payment.</param>
        /// <param name="createLoanPaymentDto">Details for creating the automatic payment for a loan.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RegistrationLoanAutomaticPayment(string userId, CreateLoanPaymentDto createLoanPaymentDto);

        /// <summary>
        /// Registers an automatic payment for an invoice based on the provided details.
        /// </summary>
        /// <param name="username">The username associated with the automatic payment.</param>
        /// <param name="paymentDto">Details for creating the automatic payment for an invoice.</param>
        /// <returns>A response containing the details of the registered automatic payment, or an error message if unsuccessful.</returns>
        Task<Response<AutomaticPaymentDetailDto>> RegistrationInvoiceAutomaticPayment(string username, CreateInvoiceAutomaticPaymentDto paymentDto);

        /// <summary>
        /// Retrieves a list of registered automatic payments for a specific user.
        /// </summary>
        /// <param name="username">The username for which to retrieve registered automatic payments.</param>
        /// <returns>A response containing the list of registered automatic payments, or an error message if unsuccessful.</returns>
        Task<Response<List<AutomaticPaymentDetailDto>>> GetRegisteredPaymentsByUserName(string username);

        /// <summary>
        /// Retrieves the details of a registered automatic payment by its ID.
        /// </summary>
        /// <param name="automaticPaymentId">The ID of the registered automatic payment.</param>
        /// <returns>A response containing the details of the registered automatic payment, or an error message if unsuccessful.</returns>
        Task<Response<AutomaticPaymentDetailDto>> GetRegisteredPaymentById(long automaticPaymentId);

        /// <summary>
        /// Cancels a registered automatic payment by its ID.
        /// </summary>
        /// <param name="automaticPaymentId">The ID of the registered automatic payment to cancel.</param>
        /// <returns>A response indicating success or failure, along with an error message if applicable.</returns>
        Task<Response<NoDataDto>> CancelRegisteredPaymentById(long automaticPaymentId);

        /// <summary>
        /// Handles the automatic payment process, initiating payments as needed.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandlePaymentProcess();
    }

}
