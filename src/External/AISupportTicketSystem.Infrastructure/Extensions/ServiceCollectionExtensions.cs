using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Infrastructure.Services;
using AISupportTicketSystem.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AISupportTicketSystem.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IRateLimitService, RateLimitService>();
        services.AddSingleton<IAiService, OpenAIService>();

        return services;
    }
}