using AISupportTicketSystem.Application.DTOs.Tickets;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.AssignTicket;

public record AssignTicketCommand(Guid TicketId, Guid AgentId) : IRequest<TicketDto>;