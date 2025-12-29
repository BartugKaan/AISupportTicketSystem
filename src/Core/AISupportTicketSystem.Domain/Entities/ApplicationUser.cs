using AISupportTicketSystem.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace AISupportTicketSystem.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    
    public string FullName => $"{FirstName} {LastName}";
}