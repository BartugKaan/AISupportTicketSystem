namespace AISupportTicketSystem.Domain.Enums;

public enum TicketStatus
{
    New = 0,
    Open = 1,
    InProgress= 2,
    AwaitingCustomer = 3,
    AwaitingThirdParty = 4,
    Escalated = 5,
    Resolved = 6,
    Closed = 7
}