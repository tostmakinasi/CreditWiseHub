using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AccountType;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Services
{
    public class AccountTypeService : IAccountTypeService
    {
        private readonly IGenericRepository<AccountType> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountTypeService(IGenericRepository<AccountType> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<AccountTypeDetailDto>> CreateAsync(CreateAccountTypeDto accountTypeDto)
        {
            var accountType = _mapper.Map<AccountType>(accountTypeDto);

            var checkExists = await _repository.AnyAsync(x => x.Name == accountType.Name);
            if(checkExists)
                return Response<AccountTypeDetailDto>.Fail("AccountType already exists.", HttpStatusCode.BadRequest, true);

            await _repository.AddAsync(accountType);
            await _unitOfWork.CommitAsync();

            var accountDetailDto = _mapper.Map<AccountTypeDetailDto>(accountType);

            return Response<AccountTypeDetailDto>.Success(accountDetailDto, HttpStatusCode.Created);
        }

        public async Task<Response<NoDataDto>> Delete(int id)
        {
            var accountType = await _repository.GetByIdAsync(id);

            if (accountType is null)
                return Response<NoDataDto>.Fail("AccountType not found.", HttpStatusCode.BadRequest, true);

            var hasAccounts = _repository.Where(x => x.Id == id && x.Accounts.Any()).Any();

            if (hasAccounts)
                return Response<NoDataDto>.Fail("There are Accounts linked to this Account Type, so they could not be deleted.", HttpStatusCode.BadRequest, true);

            accountType.IsActive = false;
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }

        public async Task<Response<List<AccountTypeDto>>> GetAll()
        {
            var accountTypes = await _repository.GetAll().ToListAsync();

            if (accountTypes is null || accountTypes.Count <= 0)
                return Response< List < AccountTypeDto >>.Fail("AccountTypes not found.", HttpStatusCode.BadRequest, true);
            
            var accountTypeDtos = _mapper.Map<List<AccountTypeDto>>(accountTypes);

            return Response < List < AccountTypeDto >> .Success(accountTypeDtos, HttpStatusCode.OK);
        }

        public async Task<Response<AccountTypeDetailDto>> GetById(int id)
        {
            var accountTypeAsQueryable = _repository.Where(x => x.Id == id);
            var accountType = await _repository.Where(x=> x.Id == id).SingleOrDefaultAsync();

            if (accountType is null)
                return Response<AccountTypeDetailDto>.Fail("AccountType not found.", HttpStatusCode.BadRequest, true);

            var accountTypeDetailDto = _mapper.Map<AccountTypeDetailDto>(accountType);

            accountTypeDetailDto.AccountsCount = accountTypeAsQueryable.Include(x=> x.Accounts).SelectMany(x=> x.Accounts).Count();

            return Response<AccountTypeDetailDto>.Success(accountTypeDetailDto, HttpStatusCode.OK);

        }

        public async Task<Response<NoDataDto>> Update(int id, UpdateAccountTypeDto accountTypeDto)
        {
            var accountType = await _repository.GetByIdAsync(id);

            if (accountType is null)
                return Response<NoDataDto>.Fail("AccountType not found.", HttpStatusCode.BadRequest, true);

            accountType.Name = !accountTypeDto.Name.IsNullOrEmpty() ? accountTypeDto.Name! : accountType.Name;

            accountType.MinimumOpeningBalance = accountTypeDto.MinimumOpeningBalance is not null ? (decimal)accountTypeDto.MinimumOpeningBalance : accountType.MinimumOpeningBalance;

            accountType.Description = !accountTypeDto.Name.IsNullOrEmpty() ? accountTypeDto.Description : accountType.Description;

            _repository.Update(accountType);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.OK);
        }
    }
}
