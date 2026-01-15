using AISupportTicketSystem.Application.DTOs.Categories;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery(bool IncludeInactive = false) : IRequest<IReadOnlyList<CategoryDto>>;