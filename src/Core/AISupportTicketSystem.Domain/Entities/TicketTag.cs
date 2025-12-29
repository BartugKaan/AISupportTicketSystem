using AISupportTicketSystem.Domain.Common;

namespace AISupportTicketSystem.Domain.Entities;

public class TicketTag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public bool IsAiGenerated { get; set; }
    
    // Relationships
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
}