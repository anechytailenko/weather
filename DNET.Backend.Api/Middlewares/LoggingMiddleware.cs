using System.Diagnostics;
namespace DNET.Backend.Api.Middleware;

public class LoggingMiddleware
{
    private  RequestDelegate _next;
    private  ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = Stopwatch.GetTimestamp();
        
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}");
        
        await _next(context);
        
        var durationInMs  = (Stopwatch.GetTimestamp() - startTime) * 1000 / (double)Stopwatch.Frequency;

        _logger.LogInformation($"Response: {context.Request.Method} {context.Request.Path} responded {context.Response.StatusCode} in {durationInMs}ms");
    }
    
}
    