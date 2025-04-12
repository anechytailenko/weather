using DNET.Backend.Api.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DNET.Backend.Api.Middleware;

public class RateLimitMiddleware : IMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly RateLimitOptions _rateLimitOptions;

    public RateLimitMiddleware(IConfiguration configuration, IOptionsSnapshot<RateLimitOptions> rateLimitOptions)
    {
        _configuration = configuration;
        _rateLimitOptions = rateLimitOptions.Value;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var redis = await ConnectionMultiplexer.ConnectAsync(_configuration.GetConnectionString("Redis")!);

        var ipAddress = context.Connection.RemoteIpAddress.ToString();
        var cacheKey = $"rate_limit_{ipAddress}_{DateTime.UtcNow.Hour}";
        
        var db = redis.GetDatabase();
        
        var rateLimit= await db.StringIncrementAsync(cacheKey);
        
        if (rateLimit == 1)
        {
            await db.KeyExpireAsync(cacheKey, TimeSpan.FromHours(1));
        }
        
        if (rateLimit > _rateLimitOptions.DefaultLimit)
        {
            context.Response.StatusCode = 429;
            return;
        }

        await next(context);
    }
}