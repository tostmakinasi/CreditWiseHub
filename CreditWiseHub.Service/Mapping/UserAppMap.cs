using AutoMapper;
using CreditWiseHub.Core.Dtos.User;
using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Service.Mapping
{
    public class UserAppMap : Profile
    {
        public UserAppMap()
        {
            CreateMap<UserDto, UserApp>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.TCKN)).ReverseMap()
                .ForMember(dest=> dest.DefaultAccountNumber, opt=> opt.MapFrom(src=> src.Accounts.Where(x=> x.AccountTypeId == 1).Select(x=> x.AccountNumber).FirstOrDefault()));
            CreateMap<CreateUserDto, UserApp>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.TCKN))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => new DateTime(src.DateOfBirth.Ticks, DateTimeKind.Utc)))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
