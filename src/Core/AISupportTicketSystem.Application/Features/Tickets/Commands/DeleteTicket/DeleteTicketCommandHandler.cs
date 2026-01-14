using AISupportTicketSystem.Application.Exceptions;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketCommandHandler> _logger;

    public DeleteTicketCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.Id);

        if (ticket == null) throw new NotFoundException("Ticket", request.Id);
        
        await _unitOfWork.Tickets.DeleteAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Ticket Deleted: {TicketNumber}", ticket.TicketNumber);

        return true;
    }
}