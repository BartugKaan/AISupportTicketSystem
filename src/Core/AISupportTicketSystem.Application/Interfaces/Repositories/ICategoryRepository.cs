using AISupportTicketSystem.Domain.Entities;

namespace AISupportTicketSystem.Application.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync();
    Task<IReadOnlyList<Category>> GetCategoriesWithSubCategoriesAsync();
}