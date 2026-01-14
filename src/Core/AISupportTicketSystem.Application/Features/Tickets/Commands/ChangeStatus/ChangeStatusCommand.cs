using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Domain.Enums;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.ChangeStatus;

public record ChangeStatusCommand(
    Guid TicketId,
    TicketStatus Status) : IRequest<TicketDto>;