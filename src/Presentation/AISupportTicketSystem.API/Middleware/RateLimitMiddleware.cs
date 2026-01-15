using System.Security.Claims;
using AISupportTicketSystem.Application.Interfaces;

namespace AISupportTicketSystem.API.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;

    private const int MaxRequestsPerMinute = 60;
    private const int MaxRequestsPerMinuteAuthenticated = 120;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }
        string rateLimitKey;
        int maxRequests;

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            rateLimitKey = $"user:{userId}";
            maxRequests = MaxRequestsPerMinuteAuthenticated;
        }
        else
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            rateLimitKey = $"ip:{ipAddress}";
            maxRequests = MaxRequestsPerMinute;
        }

        var result = await rateLimitService.CheckRateLimitAsync(rateLimitKey, maxRequests, Window);

        context.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, maxRequests - result.CurrentCount).ToString();

        if (!result.IsAllowed)
        {
            context.Response.Headers["Retry-After"] = ((int)result.RetryAfter.TotalSeconds).ToString();
            
            _logger.LogWarning("Rate limit exceeded for {Key}", rateLimitKey);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 429,
                message = "Too many requests. Please try again later.",
                retryAfter = (int)result.RetryAfter.TotalSeconds,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _next(context);
    }
}