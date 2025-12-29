namespace AISupportTicketSystem.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}