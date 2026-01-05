using AISupportTicketSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace AISupportTicketSystem.Infrastructure.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IDatabase _redisDb;
    private const string BlacklistPrefix = "token:blacklist:";
    
    
    public TokenBlacklistService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");
        var redis = ConnectionMultiplexer.Connect(connectionString!);
        _redisDb = redis.GetDatabase();
    }
    
    
    public async Task BlacklistTokenAsync(string token, TimeSpan expiry)
    {
        var key = $"{BlacklistPrefix}{token}";
        await _redisDb.StringSetAsync(key, "blacklisted", expiry);
    }

    public async Task<bool> IsBlacklistedAsync(string token)
    {
        var key = $"{BlacklistPrefix}{token}";
        return await _redisDb.KeyExistsAsync(key);
    }
}