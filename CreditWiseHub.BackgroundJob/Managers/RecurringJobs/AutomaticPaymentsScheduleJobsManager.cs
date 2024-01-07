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

        private readonly IAccountService _accountService;
        private readonly IAutomaticPaymentHistoryService _automaticPaymentHistoryService;
        private readonly IAutomaticPaymentsRegistrationRepository _paymentRepository;
        private readonly IMapper _mapper;

        public AutomaticPaymentsScheduleJobsManager(IAccountService accountService, IAutomaticPaymentsRegistrationRepository paymentRepository, IAutomaticPaymentHistoryService automaticPaymentHistoryService)
        {
            _accountService = accountService;
            _paymentRepository = paymentRepository;
            _automaticPaymentHistoryService = automaticPaymentHistoryService;
        }

        public async Task Process()
        {
            var payments = await _paymentRepository.GetPaymentsDueToday(GetLastInMonthDays());

            foreach (var payment in payments)
            {
                var dto = _mapper.Map<PaymentProcessDto>(payment);

                var accountResult = await _accountService.WithdrawMoneyForAutomaticPayment(dto);

                 await _automaticPaymentHistoryService.AddPaymentHistory(payment, accountResult);
            }
        }

        private List<int> GetLastInMonthDays()
        {
            var date = DateTime.UtcNow;
            var lastDayInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            if (date.Day <= lastDayInMonth)
                return new List<int> { date.Day };

            List<int> days = new List<int>();

            for (int i = 31; i <= date.Day; i--)
            {
                days.Add(i);
            }

            return days;
        }
    }
}
