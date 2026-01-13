using AISupportTicketSystem.Application.DTOs.Common;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Domain.Enums;
using AISupportTicketSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AISupportTicketSystem.Persistence.Repositories;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ticket?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(t => t.Customer)
            .Include(t => t.AssignedAgent)
            .Include(t => t.Category)
            .Include(t => t.Messages.OrderByDescending(m => m.CreatedAt))
            .ThenInclude(m => m.Sender)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Ticket?> GetByTicketNumberAsync(string ticketNumber)
    {
        return await _dbSet
            .Include(t => t.Customer)
            .Include(t => t.AssignedAgent)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
    }

    public async Task<PagedResult<Ticket>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TicketStatus? status = null,
        TicketPriority? priority = null,
        Guid? customerId = null,
        Guid? agentId = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true)
    {
        var query = _dbSet
            .Include(t => t.Customer)
            .Include(t => t.AssignedAgent)
            .Include(t => t.Category)
            .AsQueryable();

        // Filters
        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (customerId.HasValue)
            query = query.Where(t => t.CustomerId == customerId.Value);

        if (agentId.HasValue)
            query = query.Where(t => t.AssignedAgentId == agentId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term) ||
                t.TicketNumber.ToLower().Contains(term));
        }

        // Total count before pagination
        var totalCount = await query.CountAsync();

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "title" => sortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "status" => sortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "priority" => sortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            "createdat" => sortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        // Pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Ticket>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<string> GenerateTicketNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"TKT-{year}-";

        var lastTicket = await _dbSet
            .Where(t => t.TicketNumber.StartsWith(prefix))
            .OrderByDescending(t => t.TicketNumber)
            .FirstOrDefaultAsync();

        if (lastTicket == null)
            return $"{prefix}00001";

        var lastNumber = int.Parse(lastTicket.TicketNumber.Replace(prefix, ""));
        return $"{prefix}{(lastNumber + 1):D5}";
    }

    public async Task<int> GetOpenTicketsCountByAgentAsync(Guid agentId)
    {
        return await _dbSet.CountAsync(t =>
            t.AssignedAgentId == agentId &&
            t.Status != TicketStatus.Closed &&
            t.Status != TicketStatus.Resolved);
    }

    public async Task<IReadOnlyList<Ticket>> GetTicketsByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Include(t => t.AssignedAgent)
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}