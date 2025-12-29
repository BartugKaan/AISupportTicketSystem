using AISupportTicketSystem.Domain.Common;

namespace AISupportTicketSystem.Domain.Entities;

public class TicketMessage : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public bool IsAiGenerated { get; set; }

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;

    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();

}