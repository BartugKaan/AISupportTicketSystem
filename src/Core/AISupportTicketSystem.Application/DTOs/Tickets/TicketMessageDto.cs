namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record TicketMessageDto(
    Guid Id,
    string Content,
    bool IsInternal,
    bool IsAiGenerated,
    Guid SenderId,
    string SenderName,
    string SenderEmail,
    DateTime CratedAt
    );