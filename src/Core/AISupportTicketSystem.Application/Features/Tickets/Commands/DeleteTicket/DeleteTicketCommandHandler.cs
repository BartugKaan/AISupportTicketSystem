using AISupportTicketSystem.Application.Constants;
using AISupportTicketSystem.Application.Exceptions;
using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketCommandHandler> _logger;
    private readonly ICacheService _cacheService;

    public DeleteTicketCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteTicketCommandHandler> logger, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.Id);

        if (ticket == null)
            throw new NotFoundException("Ticket", request.Id);

        await _unitOfWork.Tickets.DeleteAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.Ticket(request.Id));

        _logger.LogInformation("Ticket deleted: {TicketNumber}", ticket.TicketNumber);

        return true;
    }
}