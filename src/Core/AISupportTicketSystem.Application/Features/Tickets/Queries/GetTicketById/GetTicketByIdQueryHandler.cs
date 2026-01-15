using AISupportTicketSystem.Application.Constants;
using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<TicketDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var cachedKey = CacheKeys.Ticket(request.Id);

        var cachedTicket = await _cacheService.GetAsync<TicketDto>(cachedKey);

        if (cachedTicket != null) return cachedTicket;

        var ticket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(request.Id);
        
        if(ticket == null) return  null;
        var ticketDto = _mapper.Map<TicketDto>(ticket);
        
        await _cacheService.SetAsync(cachedKey, ticketDto, TimeSpan.FromMinutes(5));
        return ticketDto;
    }
}