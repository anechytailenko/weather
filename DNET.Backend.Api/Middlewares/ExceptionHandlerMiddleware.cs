namespace DNET.Backend.Api.Middleware;

public class ExceptionHandlerMiddleware : IMiddleware 
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    
    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request for path: {Path}", context.Request.Path);

            context.Response.StatusCode = 500;
            
            var errorMessage = new
            {
                Message = ex.Message,
                Status = 500
            };
            
            await context.Response.WriteAsJsonAsync(errorMessage);
        }
    }
}