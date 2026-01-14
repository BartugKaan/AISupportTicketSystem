using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.DeleteTicket;

public record DeleteTicketCommand(Guid Id) : IRequest<bool>;