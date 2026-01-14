using AISupportTicketSystem.Application.DTOs.Tickets;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetMyTickets;

public record GetMyTicketsQuery(Guid UserId, bool IsAgent = false) : IRequest<IReadOnlyList<TicketListDto>>;