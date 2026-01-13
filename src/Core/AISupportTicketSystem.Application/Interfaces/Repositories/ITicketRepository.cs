using AISupportTicketSystem.Application.DTOs.Common;
using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.Interfaces.Repositories;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<Ticket?> GetByIdWithDetailsAsync(Guid id);
    Task<Ticket?> GetByTicketNumberAsync(string ticketNumber);
    Task<PagedResult<Ticket>> GetPagedAsync(
        int pagedNumber,
        int pagedSize,
        TicketStatus? status = null,
        TicketPriority? priority = null,
        Guid? customerId = null,
        Guid? agentId = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true
        );
    Task<string> GenerateTicketNumberAsync();
    Task<int> GetOpenTicketsCountByAgentAsync(Guid agentId);
    Task<IReadOnlyList<Ticket>> GetTicketsByCustomerAsync(Guid customerId);
}