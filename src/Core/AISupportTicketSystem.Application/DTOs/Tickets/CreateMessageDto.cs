namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record CreateMessageDto(
    string Content,
    bool IsInternal = false);