using AutoMapper;
using CreditWiseHub.Core.Dtos.Ticket;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Mapping
{
    public class TicketMap : Profile
    {
        public TicketMap()
        {
            CreateMap<CustomerTicket, TicketDto>()
                .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(x => x.TicketNumber, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Username, opt => opt.MapFrom(src => src.UserApp.UserName));

            CreateMap<CustomerTicket, TicketWithPriorityDto>()
                .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(x => x.TicketNumber, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Username, opt => opt.MapFrom(src => src.UserApp.UserName))
                .ForMember(x => x.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));

            CreateMap<CreateTicketDto, CustomerTicket>()
                .ForMember(x => x.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(x => x.Priority, opt => opt.MapFrom(src => TicketPriority.Normal))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => TicketStatus.Open));


        }
    }
}
