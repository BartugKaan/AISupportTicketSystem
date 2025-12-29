using AISupportTicketSystem.Domain.Common;

namespace AISupportTicketSystem.Domain.Entities;

public class TicketAttachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    
    // Relationships
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    
    public Guid? MessageId { get; set; }
    public TicketMessage? Message { get; set; }
    
    public Guid UploadedById { get; set; }
    public ApplicationUser UploadedBy { get; set; } = null!;
}