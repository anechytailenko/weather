namespace DNET.Backend.Api.Middleware;

public class ExceptionHandlerMiddleware : IMiddleware 
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
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