using AISupportTicketSystem.Application.DTOs.Common;
using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetAllTickets;

public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, PagedResult<TicketListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTicketsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<TicketListDto>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var pagedTickets = await _unitOfWork.Tickets.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Status,
            request.Priority,
            request.CustomerId,
            request.AgentId,
            request.SearchTerm,
            request.SortBy,
            request.SortDescending);
        
        var ticketDtos = _mapper.Map<IReadOnlyList<TicketListDto>>(pagedTickets.Items);
        
        return new PagedResult<TicketListDto>(
            ticketDtos,
            pagedTickets.TotalCount,
            pagedTickets.PageNumber,
            pagedTickets.PageSize);
    }
}