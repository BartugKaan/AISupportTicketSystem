namespace AISupportTicketSystem.Application.DTOs.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    string? Icon,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    IReadOnlyList<CategoryDto>? SubCategories);