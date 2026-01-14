using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Exceptions;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, TicketDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateTicketCommandHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TicketDto> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(request.Id);

        if (ticket == null) throw new NotFoundException("Ticket", request.Id);

        if (!string.IsNullOrEmpty(request.Title)) ticket.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description)) ticket.Description = request.Description;

        if (request.Priority.HasValue) ticket.Priority = request.Priority.Value;

        if (request.CategoryId.HasValue) ticket.CategoryId = request.CategoryId;

        ticket.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Ticket Updated: {TicketNumber}", ticket.TicketNumber);
        
        var updatedTicket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(ticket.Id);
        return _mapper.Map<TicketDto>(updatedTicket);
    }
}