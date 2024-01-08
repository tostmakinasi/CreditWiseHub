using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.BackgroundJob.Managers.RecurringJobs
{
    public class AutomaticPaymentsScheduleJobsManager
    {
        private readonly IAutomaticPaymentService _paymentService;

        public AutomaticPaymentsScheduleJobsManager(IAutomaticPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task Process()
        {
            await _paymentService.HandlePaymentProcess();
        }

   
    }
}
