using AISupportTicketSystem.Application.DTOs.Tickets;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.AddMessage;

public record AddMessageCommand(
    Guid TicketId,
    Guid SenderId,
    string Content,
    bool IsInternal = false) : IRequest<TicketMessageDto>;