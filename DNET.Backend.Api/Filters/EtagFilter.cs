using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNET.Backend.Api.Filters;

public class EtagFilter : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var etag = GenerateETag(context.Result);

        if (context.HttpContext.Request.Headers.TryGetValue("If-None-Match", out var value) && value.ToString() == etag)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            return;
        }
        
        context.HttpContext.Response.Headers.Append("ETag", etag);
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