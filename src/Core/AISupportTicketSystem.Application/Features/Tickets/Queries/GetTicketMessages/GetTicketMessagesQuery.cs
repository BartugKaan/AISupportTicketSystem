using AISupportTicketSystem.Application.DTOs.Tickets;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetTicketMessages;

public record GetTicketMessagesQuery(
    Guid TicketId,
    bool IncludeInternal = false) : IRequest<IReadOnlyList<TicketMessageDto>>;