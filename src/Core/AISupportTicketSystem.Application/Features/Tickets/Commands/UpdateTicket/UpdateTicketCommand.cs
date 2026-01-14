using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Domain.Enums;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public record UpdateTicketCommand(
    Guid Id,
    string? Title,
    string? Description,
    TicketPriority? Priority,
    Guid? CategoryId) : IRequest<TicketDto>;