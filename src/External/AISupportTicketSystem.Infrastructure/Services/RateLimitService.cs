using AISupportTicketSystem.Application.DTOs.RateLimit;
using AISupportTicketSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AISupportTicketSystem.Infrastructure.Services;

public class RateLimitService : IRateLimitService
{
    private readonly IDatabase _database;
    private readonly ILogger<RateLimitService> _logger;
    private const string RateLimitPrefix = "ratelimit:";

    public RateLimitService(IConfiguration configuration,IDatabase database, ILogger<RateLimitService> logger)
    {
        _logger = logger;
        var connectionString = configuration.GetConnectionString("Redis");
        var connection = ConnectionMultiplexer.Connect(connectionString!);
        _database = database;
    }

    public async Task<RateLimitResult> CheckRateLimitAsync(string key, int maxRequests, TimeSpan window)
    {
        var redisKey = $"{RateLimitPrefix}{key}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - (long)window.TotalSeconds;

        try
        {
            await _database.SortedSetRemoveRangeByScoreAsync(redisKey, 0, windowStart);
            
            var currentCount = await _database.SortedSetLengthAsync(redisKey);

            if (currentCount >= maxRequests)
            {
                var oldestRequest = await _database.SortedSetRangeByRankWithScoresAsync(redisKey,0,0);
                var retryAfter = TimeSpan.Zero;

                if (oldestRequest.Length > 0)
                {
                    var oldestTime = (long)oldestRequest[0].Score;
                    retryAfter = TimeSpan.FromSeconds(oldestTime + (long)window.TotalSeconds - now);
                }
                
                _logger.LogWarning("Rate limit exceeded for key: {Key}. Count: {Count}/{Max}", 
                    key, currentCount, maxRequests);

                return new RateLimitResult(false, (int)currentCount, maxRequests, retryAfter);
            }
            
            await _database.SortedSetAddAsync(redisKey, Guid.NewGuid().ToString(), now);
            await _database.KeyExpireAsync(redisKey, window.Add(TimeSpan.FromSeconds(10)));
            
            return new RateLimitResult(true, (int)currentCount + 1, maxRequests, TimeSpan.Zero);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for key: {Key}", key);
            return new RateLimitResult(true, 0, maxRequests, TimeSpan.Zero);
        }
    }
}