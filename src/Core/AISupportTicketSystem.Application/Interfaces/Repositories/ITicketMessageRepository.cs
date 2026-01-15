using AISupportTicketSystem.Domain.Entities;

namespace AISupportTicketSystem.Application.Interfaces.Repositories;

public interface ITicketMessageRepository : IGenericRepository<TicketMessage>
{
    Task<IReadOnlyList<TicketMessage>> GetByTicketIdAsync(Guid ticketId, bool includeInternal = false);
    Task<TicketMessage?> GetByIdWithSenderAsync(Guid id);
}