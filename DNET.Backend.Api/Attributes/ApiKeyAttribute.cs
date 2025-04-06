using DNET.Backend.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DNET.Backend.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class ApiKeyAttribute : TypeFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyFilter))
    {
    }
}