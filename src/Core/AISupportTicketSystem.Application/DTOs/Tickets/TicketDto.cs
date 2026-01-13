using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record TicketDto(
    Guid Id,
    string TicketNumber,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    TicketSource Source,
    SentimentType? Sentiment,
    string CustomerName,
    string CustomerEmail,
    Guid AssignedAgentName,
    Guid? CategoryId,
    string? CategoryName,
    DateTime? DueDate,
    DateTime? FirstResponseAt,
    DateTime? ResolvedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
    );