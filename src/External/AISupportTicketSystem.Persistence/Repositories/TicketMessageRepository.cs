using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AISupportTicketSystem.Persistence.Repositories;

public class TicketMessageRepository : GenericRepository<TicketMessage>, ITicketMessageRepository
{
    public TicketMessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TicketMessage>> GetByTicketIdAsync(Guid ticketId, bool includeInternal = false)
    {
        var query = _dbSet
            .Include(x => x.Sender)
            .Where(x => x.TicketId == ticketId);

        if (!includeInternal) 
            query = query.Where(x => !x.IsInternal);
        
        return await query
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<TicketMessage?> GetByIdWithSenderAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Sender)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}