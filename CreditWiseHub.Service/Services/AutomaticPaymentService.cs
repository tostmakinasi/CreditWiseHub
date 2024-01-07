using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Models;
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
        private readonly IGenericRepository<AutomaticPaymentRegistration,long> _registrationRepository;
        private readonly UserManager<UserApp> _userManager; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AutomaticPaymentService(IGenericRepository<AutomaticPaymentRegistration, long> registrationRepository, IUnitOfWork unitOfWork, IMapper mapper, UserManager<UserApp> userManager)
        {
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
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

            var registeredPaymentList = await _registrationRepository.Where(x => x.IsActive).Include(x => x.AutomaticPaymentHistories).ToListAsync();

            if(!registeredPaymentList.Any())
                return Response< List<AutomaticPaymentDetailDto>>.Fail("Registered Automatic Payments not found", System.Net.HttpStatusCode.NotFound, true);

            var paymentDtoList = _mapper.Map<List<AutomaticPaymentDetailDto>>(registeredPaymentList);

            return Response<List<AutomaticPaymentDetailDto>>.Success(paymentDtoList, System.Net.HttpStatusCode.OK);
        }

        public async Task<Response<AutomaticPaymentDetailDto>> RegistrationInvoiceAutomaticPayment(string username, CreateInvoiceAutomaticPaymentDto paymentDto)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return Response<AutomaticPaymentDetailDto>.Fail("User not found", System.Net.HttpStatusCode.NotFound, true);

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

   
    }
}
