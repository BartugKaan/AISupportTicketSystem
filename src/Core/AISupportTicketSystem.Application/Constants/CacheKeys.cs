namespace AISupportTicketSystem.Application.Constants;

public static class CacheKeys
{
    public const string TicketPrefix = "ticket:";
    public const string TicketListPrefix = "tickets:";
    public const string CategoryPrefix = "category:";
    public const string CategoriesAll = "categories:all";
    public const string UserPrefix = "user:";
    public const string DashboardStats = "dashboard:stats";

    public static string Ticket(Guid id) => $"{TicketPrefix}{id}";
    public static string TicketsByCustomer(Guid customerId) => $"{TicketListPrefix}customer:{customerId}";
    public static string TicketsByAgent(Guid agentId) => $"{TicketListPrefix}agent:{agentId}";
    public static string User(Guid id) => $"{UserPrefix}{id}";
}