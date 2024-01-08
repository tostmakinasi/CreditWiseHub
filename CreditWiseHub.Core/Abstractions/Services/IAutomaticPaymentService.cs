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
    public interface IAutomaticPaymentService
    {
        Task RegistrationLoanAutomaticPayment(string userId, CreateLoanPaymentDto createLoanPaymentDto);
        Task<Response<AutomaticPaymentDetailDto>> RegistrationInvoiceAutomaticPayment(string username, CreateInvoiceAutomaticPaymentDto paymentDto);
        Task<Response<List<AutomaticPaymentDetailDto>>> GetRegisteredPaymentsByUserName(string username);
        Task<Response<AutomaticPaymentDetailDto>> GetRegisteredPaymentById(long automaticPaymentId);
        Task<Response<NoDataDto>> CancelRegisteredPaymentById(long automaticPaymentId);
        Task HandlePaymentProcess();

    }
}
