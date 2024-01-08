using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Ticket;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Services
{
    public class CustomerTicketService : ICustomerTicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerTicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IMapper mapper, UserManager<UserApp> userManager)
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Response<NoDataDto>> CloseTicketByTicketNumber(long ticketNumber)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketNumber);
            if (ticket is null)
                return Response<NoDataDto>.Fail("Ticket not found", HttpStatusCode.NotFound, true);

            ticket.Status = Core.Enums.TicketStatus.Closed;
            ticket.ResolvedDate = DateTime.UtcNow;
            ticket.UpdatedDate = DateTime.UtcNow;
            ticket.IsActive = false;
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(HttpStatusCode.NoContent);
        }

        public async Task<Response<TicketDto>> CreateTicketByUsername(string username, CreateTicketDto createTicketDto)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return Response<TicketDto>.Fail("User not found", HttpStatusCode.NotFound, true);

            var ticket = _mapper.Map<CustomerTicket>(createTicketDto);

            ticket.UserAppId = user.Id;
            await _ticketRepository.AddAsync(ticket);
            await _unitOfWork.CommitAsync();
            var ticketDto = _mapper.Map<TicketDto>(ticket);
            return Response<TicketDto>.Success(ticketDto, HttpStatusCode.Created);
        }

        public async Task<Response<List<TicketWithPriorityDto>>> GetAllOpenTickets()
        {
            var tickets = await _ticketRepository.GetAllAsync(x=> x.IsActive == true && (x.Status == Core.Enums.TicketStatus.Open || x.Status == Core.Enums.TicketStatus.InProgress));

            var ticketDtoList = _mapper.Map<List<TicketWithPriorityDto>>(tickets);

            return Response<List<TicketWithPriorityDto>>.Success(ticketDtoList, HttpStatusCode.OK);
        }

        public async Task<Response<List<TicketDto>>> GetTicketByUsername(string username)
        {
            var tickets = await _ticketRepository.GetTicketsByUsername(username);

            var ticketDtoList = _mapper.Map<List<TicketDto>>(tickets);

            return Response<List<TicketDto>>.Success(ticketDtoList, HttpStatusCode.OK);
        }

        public async Task<Response<TicketWithPriorityDto>> UpgradeTicketPriorityByTicketNumber(long ticketNumber)
        {
            var ticket = await _ticketRepository.GetTicketsById(ticketNumber);
            if (ticket is null)
                return Response<TicketWithPriorityDto>.Fail("Ticket not found", HttpStatusCode.NotFound, true);

            if (ticket.Priority == TicketPriority.High)
                return Response<TicketWithPriorityDto>.Fail("Ticket already High Priority", HttpStatusCode.OK, true);

            ticket.Priority = TicketPriority.High;
            ticket.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            var ticketDto = _mapper.Map<TicketWithPriorityDto>(ticket);

            return Response<TicketWithPriorityDto>.Success(ticketDto, HttpStatusCode.OK);
        }

        public async Task<Response<TicketWithPriorityDto>> PutTicketInProgress(long ticketNumber)
        {
            var ticket = await _ticketRepository.GetTicketsById(ticketNumber);
            if (ticket is null)
                return Response<TicketWithPriorityDto>.Fail("Ticket not found", HttpStatusCode.NotFound, true);

            ticket.Status = TicketStatus.InProgress;
            await _unitOfWork.CommitAsync();

            var ticketDto = _mapper.Map<TicketWithPriorityDto>(ticket);

            return Response<TicketWithPriorityDto>.Success(ticketDto, HttpStatusCode.OK);
        }
    }
}
