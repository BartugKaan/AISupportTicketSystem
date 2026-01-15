using AISupportTicketSystem.Application.DTOs.RateLimit;

namespace AISupportTicketSystem.Application.Interfaces;

public interface IRateLimitService
{
    Task<RateLimitResult> CheckRateLimitAsync(string key, int maxRequests, TimeSpan window);
}