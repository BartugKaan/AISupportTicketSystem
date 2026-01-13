using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record UpdateTicketDto(
    string? Title,
    string? Description,
    TicketPriority? Priority,
    Guid? CategoryId);