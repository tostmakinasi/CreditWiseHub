using AutoMapper;
using CreditWiseHub.Core.Dtos.LoanType;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Mapping
{
    public class LoanMap : Profile
    {
        public LoanMap()
        {
            CreateMap<LoanType, LoanTypeDto>();
            CreateMap<CreateLoanTypeDto, LoanType>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateLoanTypeDto, LoanType>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
