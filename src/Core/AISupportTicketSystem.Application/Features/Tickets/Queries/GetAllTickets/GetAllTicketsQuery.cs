using AISupportTicketSystem.Application.DTOs.Common;
using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Domain.Enums;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetAllTickets;

public record GetAllTicketsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    Guid? CustomerId = null,
    Guid? AgentId = null,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = true
) : IRequest<PagedResult<TicketListDto>>;