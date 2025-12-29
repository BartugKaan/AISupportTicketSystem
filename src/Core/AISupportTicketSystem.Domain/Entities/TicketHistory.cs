using AISupportTicketSystem.Domain.Common;

namespace AISupportTicketSystem.Domain.Entities;

public class TicketHistory : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
    
    // Relationships
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    
    public Guid? PerformedById { get; set; }
    public ApplicationUser? PerformedBy { get; set; }
}