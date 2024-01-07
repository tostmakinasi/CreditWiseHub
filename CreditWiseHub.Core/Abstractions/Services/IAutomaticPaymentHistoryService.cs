using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Services
{
    public interface IAutomaticPaymentHistoryService
    {
        Task AddPaymentHistory(AutomaticPaymentRegistration paymnet, Response<PaymentProcessResultDto> processResult);
    }
}
