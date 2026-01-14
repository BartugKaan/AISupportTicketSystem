using AISupportTicketSystem.Application.DTOs.Tickets;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetTicketById;

public record GetTicketByIdQuery(Guid Id) : IRequest<TicketDto?>;