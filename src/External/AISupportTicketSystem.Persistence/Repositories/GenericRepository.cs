using System.Linq.Expressions;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AISupportTicketSystem.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync(); 
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
    {
        return predicate == null
            ? _dbSet.CountAsync()
            : _dbSet.CountAsync(predicate);
    }
}