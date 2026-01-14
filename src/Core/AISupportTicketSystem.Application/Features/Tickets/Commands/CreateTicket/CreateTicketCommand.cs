using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Domain.Enums;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority? Priority,
    TicketSource? Source,
    Guid? CategoryId,
    Guid CustomerId) : IRequest<TicketDto>;