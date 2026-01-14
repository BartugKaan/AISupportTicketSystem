using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Exceptions;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.ChangeStatus;

public class ChangeStatusCommandHandler : IRequestHandler<ChangeStatusCommand, TicketDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeStatusCommandHandler> _logger;

    public ChangeStatusCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ChangeStatusCommandHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TicketDto> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(request.TicketId);

        if (ticket == null) throw new NotFoundException("Ticket", request.TicketId);

        var oldStatus = ticket.Status;
        ticket.Status = request.Status;
        ticket.UpdatedAt = DateTime.UtcNow;

        switch (request.Status)
        {
            case TicketStatus.Resolved:
                ticket.ResolvedAt = DateTime.UtcNow;
                break;
            case TicketStatus.Closed:
                ticket.ClosedAt = DateTime.UtcNow;
                break;
        }
        
        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Ticket {TicketNumber} status changed from {OldStatus} to {NewStatus}.}", ticket.TicketNumber, oldStatus, request.Status);
        return _mapper.Map<TicketDto>(ticket);
    }
}