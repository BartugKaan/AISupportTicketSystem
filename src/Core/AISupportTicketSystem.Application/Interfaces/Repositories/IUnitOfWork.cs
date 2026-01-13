namespace AISupportTicketSystem.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    ITicketRepository Tickets { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}