using AISupportTicketSystem.Application.Interfaces;

namespace AISupportTicketSystem.API.Middleware;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenBlacklistMiddleware> _logger;

    public TokenBlacklistMiddleware(RequestDelegate next, ILogger<TokenBlacklistMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService tokenBlacklistService)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "");

            if (await tokenBlacklistService.IsBlacklistedAsync(token))
            {
                _logger.LogInformation("Blacklisted token used. Token {Token}", token[..20] + "..." );
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Request.ContentType = "application/json";

                var response = new
                {
                    status = 401,
                    message = "The token you used is invalid. Please login Again!",
                    timestamp = DateTime.UtcNow
                };
                
                await context.Response.WriteAsJsonAsync(response);
                return;
            }
        }
        await  _next(context);
    }
}