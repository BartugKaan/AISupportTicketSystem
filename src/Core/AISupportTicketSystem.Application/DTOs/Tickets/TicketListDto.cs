using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record TicketListDto(
    Guid Id,
    string TicketNumber,
    string Title,
    TicketStatus Status,
    TicketPriority Priority,
    SentimentType? Sentiment,
    string CustomerName,
    string? AssignedAgentName,
    string CategoryName,
    DateTime? CreatedAt);