using AutoMapper;
using CreditWiseHub.Core.Dtos.LoanApplication;
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

            CreateMap<CreateLoanApplicationDto, LoanApplication>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<LoanApplication, LoanApplicationStatusDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsRejected.HasValue ? ((bool)src.IsRejected ? "Reddedildi" : "Onaylandı") : "Başvuru süreciniz devam ediyor."))
                .ForMember(dest => dest.ApplicationNumber, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.ApprovalDate.HasValue ? DateTime.SpecifyKind((DateTime)src.ApprovalDate, DateTimeKind.Local).ToShortDateString() : ""));

            CreateMap<LoanApplication, LoanApplicationListDto>()
                .ForMember(x => x.ApplicationDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.CreatedDate, DateTimeKind.Local).ToShortDateString()))
                .ForMember(x => x.ApplicationNumber, opt => opt.MapFrom(src => src.Id));
        }
    }
}
