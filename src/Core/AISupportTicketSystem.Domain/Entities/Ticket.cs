using AISupportTicketSystem.Domain.Common;
using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Domain.Entities;

public class Ticket : AuditableEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Status & Priority
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketSource Source { get; set; } = TicketSource.Web;
    
    // AI tarafından belirlenen alanlar
    public SentimentType? Sentiment { get; set; }
    public double? SentimentScore { get; set; }
    public string? AiSuggestedCategory { get; set; }
    public string? AiSuggestedResponse { get; set; }
    
    // Vector embedding (pgvector için)
    public float[]? Embedding { get; set; }
    
    // Relationships
    public Guid CustomerId { get; set; }
    public ApplicationUser Customer { get; set; } = null!;
    
    public Guid? AssignedAgentId { get; set; }
    public ApplicationUser? AssignedAgent { get; set; }
    
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    // SLA & Timestamps
    public DateTime? DueDate { get; set; }
    public DateTime? FirstResponseAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    // Collections
    public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    public ICollection<TicketTag> Tags { get; set; } = new List<TicketTag>();
    public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();
}