namespace AISupportTicketSystem.Application.DTOs.RateLimit;

public record RateLimitResult(
    bool IsAllowed,
    int CurrentCount,
    int MaxRequests,
    TimeSpan RetryAfter);