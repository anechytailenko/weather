using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNET.Backend.Api.Filters;

public class EtagFilter : ResultFilterAttribute
{

    private readonly ILogger<EtagFilter> _logger;
    
    public EtagFilter(ILogger<EtagFilter> logger)
    {
        _logger = logger;
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var etag = GenerateETag(context.Result);

        if (context.HttpContext.Request.Headers.TryGetValue("If-None-Match", out var value) && value.ToString() == etag)
        {

            _logger.LogInformation("ETag match for {Path}. Returned: Status 304 Not Modified.", context.HttpContext.Request.Path);
            context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            return;
        }
        _logger.LogDebug("Generating new ETag for {Path}: {ETag}", context.HttpContext.Request.Path, etag);

    }
    
    private static string GenerateETag(IActionResult response)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(response);
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        var eTag = Convert.ToBase64String(hash);
        return eTag;
    }
}