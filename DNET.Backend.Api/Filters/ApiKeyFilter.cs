using DNET.Backend.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DNET.Backend.Api.Filters;

public class ApiKeyFilter : IAsyncActionFilter
{
    private readonly IOptionsSnapshot<ApiKeyOptions> _config;
    private const string ApiKeyHeaderName = "X-API-KEY";

    public ApiKeyFilter(IOptionsSnapshot<ApiKeyOptions> config)
    {
        _config = config;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
        {
            await next();
            return;
        }
        
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Missed API Key");
            return;
        }
        

        var validApiKeys = _config.Value.ValidApiKeys;

        if ( !validApiKeys.ContainsValue(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }

        await next();
    }
}