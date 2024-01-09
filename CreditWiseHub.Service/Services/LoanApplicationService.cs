using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Loan;
using CreditWiseHub.Core.Dtos.LoanApplication;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using CreditWiseHub.Service.Exceptions;
using CreditWiseHub.Service.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CreditWiseHub.Service.Services
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IGenericRepository<LoanType, int> _loanTypeRepository;
        private readonly IAutomaticPaymentService _automaticPaymentService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LoanApplicationService(ILoanApplicationRepository loanApplicationRepository, IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, IAutomaticPaymentService automaticPaymentService, IGenericRepository<LoanType, int> repository, UserManager<UserApp> userManager)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _accountService = accountService;
            _automaticPaymentService = automaticPaymentService;
            _loanTypeRepository = repository;
            _userManager = userManager;
        }

        public async Task<Response<NoDataDto>> ApproveLoanApplication(long applicationNumber, string ApproverId)
        {
            var application = await ProcessApplication(applicationNumber, true, ApproverId);

            await _unitOfWork.BeginTransactionAsync();

            var transactionResponse = await _accountService.TransferCreditToAccountByUserId(application.UserId, application.RequestedAmount);

            application.TransactionId = transactionResponse.data.TransactionId;
            await _unitOfWork.CommitAsync();

            using LoanHelper helper = new LoanHelper(application.RequestedAmount, application.LoanType.InterestRate, application.InstallmentCount, application.ApprovalDate.Value);

            CreateLoanPaymentDto createLoanPaymentDto = new()
            {
                Name = application.LoanType.Name,
                PaymentAmount = (decimal)helper.MonthlyInstallmentCalculate(),
                PaymentDueDay = application.ApprovalDate.Value.Day,
                PaymentDueCount = application.InstallmentCount
            };

            await _automaticPaymentService.RegistrationLoanAutomaticPayment(application.UserId, createLoanPaymentDto);

            await _unitOfWork.TransactionCommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        public async Task<Response<List<LoanApplicationListDto>>> GetWaitingLoanApplicationsAsync()
        {
            var applications = await _loanApplicationRepository.Where(x => !x.IsRejected.HasValue && x.IsActive == true).Include(x=> x.LoanType).ToListAsync();

            if (!applications.Any())
                return Response<List<LoanApplicationListDto>>.Fail("There are no pending applications", HttpStatusCode.NotFound, true);

            var applicationDtoList = _mapper.Map<List<LoanApplicationListDto>>(applications);

            return Response<List<LoanApplicationListDto>>.Success(applicationDtoList, HttpStatusCode.OK);
        }

        public async Task<Response<NoDataDto>> RejectLoanApplication(long applicationNumber, string ApproverId)
        {
            await ProcessApplication(applicationNumber, false, ApproverId);

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        public async Task<Response<List<PaymentPlanDto>>> GetLoanPaymentPlanByApplicationNumber(long applicationNumber)
        {
            var loanApplication = await _loanApplicationRepository.GetWithLoanTypeByApplicationNumber(applicationNumber);

            if (loanApplication is null)
                return Response<List<PaymentPlanDto>>.Fail("Application not found", HttpStatusCode.NotFound, true);

            if (!loanApplication.IsRejected.HasValue && (loanApplication.IsRejected == true || loanApplication.IsApproved == false))
                return Response<List<PaymentPlanDto>>.Fail("Application not approved", HttpStatusCode.NotAcceptable, true);

            using LoanHelper helper = new LoanHelper(loanApplication.RequestedAmount, loanApplication.LoanType.InterestRate, loanApplication.InstallmentCount, loanApplication.ApprovalDate.Value);

            List<PaymentPlanDto> paymentPlan = new();

            foreach (var payment in helper.CalculateLoanPayment())
            {
                paymentPlan.Add(payment);
            }

            return Response<List<PaymentPlanDto>>.Success(paymentPlan, HttpStatusCode.OK);
        }

        private async Task<LoanApplication> ProcessApplication(long applicationNumber, bool isApproved, string ApproverId )
        {
            var application = await _loanApplicationRepository.GetWithLoanTypeByApplicationNumber(applicationNumber);

            if (application is null)
                throw new NotFoundException("Application not found");

            application.IsRejected = !isApproved;
            application.IsApproved = isApproved;
            application.ApprovalDate = DateTime.UtcNow;
            application.ApproverId = ApproverId;
            application.IsActive = false;
            await _unitOfWork.CommitAsync();

            return application;
        }

        public async Task<Response<LoanApplicationStatusDto>> ApplyLoanByUsername(string username, CreateLoanApplicationDto createLoanApplicationDto)
        {
            var loanType = await _loanTypeRepository.GetByIdAsync(createLoanApplicationDto.LoanTypeId);
            if (loanType is null)
                return Response<LoanApplicationStatusDto>.Fail("Loan Type not found", HttpStatusCode.BadRequest, true);

            if(createLoanApplicationDto.RequestedAmount < loanType.MinLoanAmount || createLoanApplicationDto.RequestedAmount > loanType.MaxLoanAmount)
                return Response<LoanApplicationStatusDto>.Fail("Requested Amount must be in Loan Type Max and Min range", HttpStatusCode.BadRequest, true);

            if (createLoanApplicationDto.InstallmentCount < loanType.MinInstallmentOption || createLoanApplicationDto.InstallmentCount > loanType.MaxInstallmentOption)
                return Response<LoanApplicationStatusDto>.Fail("InstallmentCount must be in Loan Type Max and Min range", HttpStatusCode.BadRequest, true);

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return Response<LoanApplicationStatusDto>.Fail("User not found", HttpStatusCode.BadRequest, true);

            var applicationsCheck = await _loanApplicationRepository.AnyAsync(x => x.UserId == user.Id && x.LoanTypeId == loanType.Id && x.IsActive);
            if(applicationsCheck)
                return Response<LoanApplicationStatusDto>.Fail("If you already have an ongoing application, wait for it to be finalized for the new application.", HttpStatusCode.NotAcceptable, true);

            var application = _mapper.Map<LoanApplication>(createLoanApplicationDto);
            application.UserId = user.Id;
            application.CreatedDate = DateTime.UtcNow;
            application.IsActive = true;
            await _loanApplicationRepository.AddAsync(application);
            await _unitOfWork.CommitAsync();

            var applicationStatus = _mapper.Map<LoanApplicationStatusDto>(application);

            return Response<LoanApplicationStatusDto>.Success(applicationStatus, HttpStatusCode.OK);
        }

        public async Task<Response<LoanApplicationStatusDto>> GetApplicationStatusByApplicationNumber(long applicationNumber)
        {
            var application = await _loanApplicationRepository.GetByIdAsync(applicationNumber);

            if (application is null)
                return Response<LoanApplicationStatusDto>.Fail("Application not found", HttpStatusCode.NotFound, true);

            var statusDto = _mapper.Map<LoanApplicationStatusDto>(application);

            return Response<LoanApplicationStatusDto>.Success(statusDto, HttpStatusCode.OK);
        }
    }
}
