using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.LoanType;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using CreditWiseHub.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Services
{
    public class LoanTypeService : ILoanTypeService
    {
        private readonly IGenericRepository<LoanType, int> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LoanTypeService(IGenericRepository<LoanType, int> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<LoanTypeDto>> AddLoanTypeAsync(CreateLoanTypeDto createLoanTypeDto)
        {
            var loanType = _mapper.Map<LoanType>(createLoanTypeDto);

            await _repository.AddAsync(loanType);
            await _unitOfWork.CommitAsync();

            var loanTypeDto = _mapper.Map<LoanTypeDto>(loanType);

            return Response<LoanTypeDto>.Success(loanTypeDto, HttpStatusCode.Created);
        }

        public async Task<Response<NoDataDto>> DeleteLoanTypeAsync(int loanTypeId)
        {
            var loanType = await GetById(loanTypeId);

            _repository.Remove(loanType);

            return Response<NoDataDto>.Success(HttpStatusCode.NoContent);
        }

        public async Task<Response<List<LoanTypeDto>>> GetAllLoanTypesAsync()
        {
            var loanTypes = await _repository.GetAllAsync(x => x.IsActive == true);

            if (loanTypes is null || loanTypes.Count() == 0)
                return Response<List<LoanTypeDto>>.Fail("LoanTypes not found", HttpStatusCode.NotFound, true);
            var loanTypeDtoList = _mapper.Map<List<LoanTypeDto>>(loanTypes);

            return Response<List<LoanTypeDto>>.Success(loanTypeDtoList, HttpStatusCode.OK);
        }

        public async Task<Response<LoanTypeDto>> GetLoanTypeByIdAsync(int loanTypeId)
        {
            var loanType = await GetById(loanTypeId);
            var loanTypeDto = _mapper.Map<LoanTypeDto>(loanType);

            return Response<LoanTypeDto>.Success(loanTypeDto, HttpStatusCode.OK);
        }

        public async Task<Response<LoanTypeDto>> UpdateLoanTypeAsync(int loanTypeId, UpdateLoanTypeDto updateLoanTypeDto)
        {
            var loanType = await _repository.GetByIdAsync(loanTypeId);

            loanType.Name = updateLoanTypeDto.Name ?? loanType.Name;
            loanType.MaxInstallmentOption = updateLoanTypeDto.MaxInstallmentOption ?? loanType.MaxInstallmentOption;
            loanType.MinInstallmentOption = updateLoanTypeDto.MinInstallmentOption ?? loanType.MinInstallmentOption;
            loanType.MinLoanAmount = updateLoanTypeDto.MinLoanAmount ?? loanType.MinLoanAmount;
            loanType.MaxLoanAmount = updateLoanTypeDto.MaxLoanAmount ?? loanType.MaxLoanAmount;
            loanType.MinCreditScore = updateLoanTypeDto.MinCreditScore ?? loanType.MinCreditScore;
            loanType.MaxCreditScore = updateLoanTypeDto.MaxCreditScore ?? loanType.MaxCreditScore;
            loanType.UpdatedDate = DateTime.UtcNow;

            _repository.Update(loanType);
            await _unitOfWork.CommitAsync();

            var loanTypeDto = _mapper.Map<LoanTypeDto>(loanType);

            return Response<LoanTypeDto>.Success(loanTypeDto, HttpStatusCode.OK);
        }

        private async Task<LoanType> GetById(int id)
        {
            var loanType = await _repository.GetByIdAsync(id);

            if (loanType is null)
                throw new NotFoundException("LoanType not found");

            return loanType;
        }
    }
}
