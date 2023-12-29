using AutoMapper;
using CreditWiseHub.Core.Dtos.AccountType;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Mapping
{
    public class AccountTypeMap : Profile
    {
        public AccountTypeMap()
        {
            CreateMap<CreateAccountTypeDto, AccountType>();
            CreateMap<AccountType, AccountTypeDetailDto>();
            CreateMap<AccountType,AccountTypeDto>();
            CreateMap<AccountType, AccountTypeDetailDto>()
                .ForMember(dest=> dest.AccountsCount, opt=> opt.Ignore());

        }
    }
}
