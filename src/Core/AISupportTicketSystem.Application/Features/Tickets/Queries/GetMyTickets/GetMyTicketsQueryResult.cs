using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetMyTickets;

public class GetMyTicketsQueryResult : IRequestHandler<GetMyTicketsQuery, IReadOnlyList<TicketListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyTicketsQueryResult(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TicketListDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Ticket> tickets;
        
        if (request.IsAgent)
        {
            tickets = await _unitOfWork.Tickets.FindAsync(t => t.AssignedAgentId == request.UserId);
        }
        else
        {
            tickets = await _unitOfWork.Tickets.GetTicketsByCustomerAsync(request.UserId);
        }
        return _mapper.Map<IReadOnlyList<TicketListDto>>(tickets);
    }
}