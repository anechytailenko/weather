using Microsoft.AspNetCore.Mvc.Filters;

namespace DNET.Backend.Api.Filters;

public class TimestampFilter : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.Append("X-Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}