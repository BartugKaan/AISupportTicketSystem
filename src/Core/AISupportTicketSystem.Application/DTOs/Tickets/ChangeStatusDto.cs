using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record ChangeStatusDto(TicketStatus Status);