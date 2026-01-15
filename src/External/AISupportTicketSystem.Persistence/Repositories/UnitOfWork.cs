using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Persistence.Context;

namespace AISupportTicketSystem.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public ITicketRepository? _tickets;
    public ICategoryRepository? _categories;
    public ITicketMessageRepository? _ticketMessages;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ITicketRepository Tickets  => _tickets ??= new TicketRepository(_dbContext);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_dbContext);
    public ITicketMessageRepository TicketMessages => _ticketMessages ??= new TicketMessageRepository(_dbContext);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}