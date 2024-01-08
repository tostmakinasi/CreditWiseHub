using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using CreditWiseHub.Service.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Services
{
    public class AutomaticPaymentService : IAutomaticPaymentService
    {
        private readonly IAccountService _accountService;
        private readonly IAutomaticPaymentHistoryService _automaticPaymentHistoryService;
        private readonly IAutomaticPaymentsRegistrationRepository _registrationRepository;
        private readonly UserManager<UserApp> _userManager; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AutomaticPaymentService(IAutomaticPaymentsRegistrationRepository registrationRepository, IUnitOfWork unitOfWork, IMapper mapper, UserManager<UserApp> userManager, IAccountService accountService, IAutomaticPaymentHistoryService automaticPaymentHistoryService)
        {
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _accountService = accountService;
            _automaticPaymentHistoryService = automaticPaymentHistoryService;
        }

        public async Task<Response<NoDataDto>> CancelRegisteredPaymentById(long automaticPaymentId)
        {
            var registeredPayment = await _registrationRepository.GetByIdAsync(automaticPaymentId);

            if (registeredPayment is null)
                return Response<NoDataDto>.Fail("Registered Automatic Payment not found", System.Net.HttpStatusCode.NotFound, true);

            if(registeredPayment.BelongToSystem)
                return Response<NoDataDto>.Fail("Automatic payment cannot be cancelled, contact customer service.", System.Net.HttpStatusCode.NotFound, true);


            registeredPayment.IsActive = false;
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(System.Net.HttpStatusCode.NoContent);

        }

        public async Task<Response<AutomaticPaymentDetailDto>> GetRegisteredPaymentById(long automaticPaymentId)
        {
            var registeredPayment = await _registrationRepository.GetByIdAsync(automaticPaymentId);

            if (registeredPayment is null)
                return Response<AutomaticPaymentDetailDto>.Fail("Registered Automatic Payment not found", System.Net.HttpStatusCode.NotFound, true);

            var paymentDto = _mapper.Map<AutomaticPaymentDetailDto>(registeredPayment);

            return Response<AutomaticPaymentDetailDto>.Success(paymentDto, System.Net.HttpStatusCode.OK);
        }

        public async Task<Response<List<AutomaticPaymentDetailDto>>> GetRegisteredPaymentsByUserName(string username)
        {
            var userId = await CheckUserAndGetUserId(username);

            var registeredPaymentList = await _registrationRepository.GetPaymentsWithHistoriesByUserId(userId);

            if(!registeredPaymentList.Any())
                return Response< List<AutomaticPaymentDetailDto>>.Fail("Registered Automatic Payments not found", System.Net.HttpStatusCode.NotFound, true);

            var paymentDtoList = _mapper.Map<List<AutomaticPaymentDetailDto>>(registeredPaymentList);

            return Response<List<AutomaticPaymentDetailDto>>.Success(paymentDtoList, System.Net.HttpStatusCode.OK);
        }

        public async Task HandlePaymentProcess()
        {
            var payments = await _registrationRepository.GetPaymentsDueToday(GetLastInMonthDays());

            foreach (var payment in payments)
            {
                payment.PaymentDuePaidCount += 1;
                await _unitOfWork.CommitAsync();

                var dto = _mapper.Map<PaymentProcessDto>(payment);

                var accountResult = await _accountService.WithdrawMoneyForAutomaticPayment(dto);

                await _automaticPaymentHistoryService.AddPaymentHistory(payment, accountResult);

                if (payment.PaymentDueCount == payment.PaymentDuePaidCount)
                {
                    payment.IsActive = false;
                    await _unitOfWork.CommitAsync();
                }
            }
        }


        public async Task<Response<AutomaticPaymentDetailDto>> RegistrationInvoiceAutomaticPayment(string username, CreateInvoiceAutomaticPaymentDto paymentDto)
        {

            var registration = _mapper.Map<AutomaticPaymentRegistration>(paymentDto);
            registration.UserId = await CheckUserAndGetUserId(username);

            await _registrationRepository.AddAsync(registration);
            await _unitOfWork.CommitAsync();

            var paymentDetailDto = _mapper.Map<AutomaticPaymentDetailDto>(registration);

            return Response<AutomaticPaymentDetailDto>.Success(paymentDetailDto, System.Net.HttpStatusCode.Created);
        }

        public async Task RegistrationLoanAutomaticPayment(string userId, CreateLoanPaymentDto createLoanPaymentDto)
        {
            var registration = _mapper.Map<AutomaticPaymentRegistration>(createLoanPaymentDto);
            registration.UserId = userId;

            await _registrationRepository.AddAsync(registration);
            await _unitOfWork.CommitAsync();
        }

        private async Task<string> CheckUserAndGetUserId(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                throw new NotFoundException("User not found");

            return user.Id;
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
