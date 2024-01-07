using AutoMapper;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Mapping
{
    public class AutomaticPaymentMap : Profile
    {
        public AutomaticPaymentMap()
        {
            CreateMap<CreateLoanPaymentDto, AutomaticPaymentRegistration>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => PaymentType.Loan))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.BelongToSystem, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => "Otomatik Kredi Taksit Tahsilatı"));

            CreateMap<CreateInvoiceAutomaticPaymentDto, AutomaticPaymentRegistration>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => PaymentType.Invoice))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.BelongToSystem, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PaymentName));

            CreateMap<AutomaticPaymentRegistration, AutomaticPaymentDetailDto>()
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType.ToString()));

            CreateMap<AutomaticPaymentHistory, PaymentHistoryDto>();

            CreateMap<AutomaticPaymentRegistration, PaymentProcessDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.PaymentType == PaymentType.Loan? $"Ödeme Talimatı İşlemi : {src.Name} {src.PaymentDuePaidCount + 1}.Taksit ödemesi" : $"Ödeme Talimatı İşlemi : {src.Name} Fatura ödemesi"));
        }
    }
}
