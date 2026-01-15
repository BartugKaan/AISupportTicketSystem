using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Exceptions;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.AddMessage;

public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, TicketMessageDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddMessageCommandHandler> _logger;

    public AddMessageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddMessageCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TicketMessageDto> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);

        if (ticket == null) throw new NotFoundException("Ticket", request.TicketId);

        var message = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            SenderId = request.SenderId,
            Content = request.Content,
            IsInternal = request.IsInternal,
            IsAiGenerated = false,
            CreatedAt =  DateTime.UtcNow,
        };

        if (ticket.FirstResponseAt == null && ticket.CustomerId != request.SenderId)
        {
            ticket.FirstResponseAt = DateTime.UtcNow;
        }

        if (ticket.CustomerId == request.SenderId && ticket.Status == TicketStatus.AwaitingCustomer)
        {
            ticket.Status = TicketStatus.Open;
        }

        if (ticket.CustomerId != request.SenderId && ticket.Status == TicketStatus.New)
        {
            ticket.Status = TicketStatus.InProgress;
        }

        ticket.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.TicketMessages.AddAsync(message);
        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Message added to ticket {TicketId} by user {SenderId}", request.TicketId, request.SenderId);

        var createdMessage = await _unitOfWork.TicketMessages.GetByIdWithSenderAsync(message.Id);
        return _mapper.Map<TicketMessageDto>(createdMessage);
    }
}