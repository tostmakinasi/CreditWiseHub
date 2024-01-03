using AutoMapper;
using CreditWiseHub.Core.Dtos.AccountType;
using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Service.Mapping
{
    public class AccountTypeMap : Profile
    {
        public AccountTypeMap()
        {
            CreateMap<CreateAccountTypeDto, AccountType>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<AccountType, AccountTypeDetailDto>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.CreatedDate, DateTimeKind.Local)));
            CreateMap<AccountType, AccountTypeDto>();
            CreateMap<AccountType, AccountTypeDetailDto>()
                .ForMember(dest => dest.AccountsCount, opt => opt.Ignore());

        }
    }
}
