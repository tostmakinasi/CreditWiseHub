using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;


namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IAutomaticPaymentHistoryService
    {
        Task AddPaymentHistory(AutomaticPaymentRegistration paymnet, Response<PaymentProcessResultDto> processResult);
    }
}
