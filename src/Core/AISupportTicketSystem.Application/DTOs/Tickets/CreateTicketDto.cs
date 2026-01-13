using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record CreateTicketDto(
    string Title,
    string Description,
    TicketPriority? Priority,
    TicketSource? Source,
    Guid? CategoryId);