using AutoMapper;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Models;
namespace CreditWiseHub.Service.Mapping
{
    public class TransactionMap : Profile
    {
        public TransactionMap()
        {
            CreateMap<Transaction, TransactionStatusDto>()
                .ForMember(dest => dest.TransactionStatus, opt => opt.MapFrom(src => src.IsConfirmed ? "Completed" : "Waiting for approval"))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TransactionStatusDto, PaymentProcessResultDto>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.TransactionDate));
        }
    }
}
