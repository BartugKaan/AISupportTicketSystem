using AISupportTicketSystem.Application.Constants;
using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Exceptions;
using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignTicketCommandHandler> _logger;
    private readonly ICacheService _cacheService;

    public AssignTicketCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AssignTicketCommandHandler> logger, ICacheService cacheService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<TicketDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(request.TicketId);

        if (ticket == null)
            throw new NotFoundException("Ticket", request.TicketId);

        ticket.AssignedAgentId = request.AgentId;
        
        if (ticket.Status == TicketStatus.New)
            ticket.Status = TicketStatus.Open;

        ticket.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.Ticket(request.TicketId));

        _logger.LogInformation("Ticket {TicketNumber} assigned to Agent {AgentId}", 
            ticket.TicketNumber, request.AgentId);

        var updatedTicket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(ticket.Id);
        return _mapper.Map<TicketDto>(updatedTicket);
    }
}