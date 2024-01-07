﻿using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Services
{
    public class AutomaticPaymentHistoryService : IAutomaticPaymentHistoryService
    {
        private readonly IGenericRepository<AutomaticPaymentHistory, long> _paymentHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AutomaticPaymentHistoryService(IGenericRepository<AutomaticPaymentHistory, long> paymentHistoryRepository, IUnitOfWork unitOfWork)
        {
            _paymentHistoryRepository = paymentHistoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddPaymentHistory(AutomaticPaymentRegistration paymnet, Response<PaymentProcessResultDto> processResult)
        {
            AutomaticPaymentHistory history = new()
            {
                RegistrationId = paymnet.Id,
                PaymentAmount = paymnet.PaymentAmount,
            };

            if (processResult.IsSuccess)
            {
                history.Comment = "Ödeme alındı";
                history.PaymentDate = processResult.Data.PaymentDate;
                history.IsPaid = true;
                history.TransactionId = processResult.Data.TransactionId;
            }
            else
            {
                history.Comment = $"Ödeme başarısız. Sebebi : {processResult.Error.Errors[0]}";
                history.PaymentDate = DateTime.UtcNow;
                history.IsPaid = false;
                history.TransactionId = null;
            }

            await _paymentHistoryRepository.AddAsync(history);
            await _unitOfWork.CommitAsync();
        }
    }
}
