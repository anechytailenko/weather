using DNET.Backend.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DNET.Backend.Api.Filters;

public class ApiKeyFilter : IAsyncActionFilter
{
    private readonly IOptionsSnapshot<ApiKeyOptions> _config;
    private const string ApiKeyHeaderName = "X-API-KEY";
    private readonly ILogger<ApiKeyFilter> _logger;

    public ApiKeyFilter(IOptionsSnapshot<ApiKeyOptions> config, ILogger<ApiKeyFilter> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
        {
            _logger.LogDebug("Skipping API key check for anonymous endpoint: {Path}", context.HttpContext.Request.Path);
            await next();
            return;
        }
        
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("API key was missed, {Path}", context.HttpContext.Request.Path);
            context.Result = new UnauthorizedObjectResult("Missed API Key");
            return;
        }
        

        var validApiKeys = _config.Value.ValidApiKeys;

        if ( !validApiKeys.ContainsValue(extractedApiKey))
        {
            _logger.LogWarning("Invalid API key was provided  to {Path}",  context.HttpContext.Request.Path);
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }
        
        _logger.LogInformation("Valid API key was provided to {Path} (IP: {IP} ", context.HttpContext.Request.Path,context.HttpContext.Connection.RemoteIpAddress);

        await next();
    }
}