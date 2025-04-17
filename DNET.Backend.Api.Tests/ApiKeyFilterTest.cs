using DNET.Backend.Api.Attributes;
using DNET.Backend.Api.Filters;
using DNET.Backend.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DNET.Backend.Api.Tests.Filters
{
    public class ApiKeyFilterTests
    {
        private readonly Mock<IOptionsSnapshot<ApiKeyOptions>> _optionMock;
        private readonly ApiKeyFilter _filter;
        private readonly ActionContext _actionContext;
        private readonly ActionExecutingContext _executingContext;
        private readonly ActionExecutionDelegate _next;

        public ApiKeyFilterTests()
        {
            _optionMock = new Mock<IOptionsSnapshot<ApiKeyOptions>>();
            
            _filter = new ApiKeyFilter(_optionMock.Object);
            
            var httpContext = new DefaultHttpContext();
            
            _actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());
            
            _executingContext = new ActionExecutingContext(
                _actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Moq.Mock.Of<Controller>());
            
            _next = () => Task.FromResult(new ActionExecutedContext(
                _actionContext,
                new List<IFilterMetadata>(),
                Moq.Mock.Of<Controller>()));
        }
        

        [Fact]
        public async Task MissedApiKey_ReturnsUnauthorized()
        {
            await _filter.OnActionExecutionAsync(_executingContext, _next);
            
            Assert.IsType<UnauthorizedObjectResult>(_executingContext.Result);
            
            Assert.Equal("Missed API Key", 
                ((UnauthorizedObjectResult)_executingContext.Result).Value);
        }

        [Fact]
        public async Task InvalidApiKey_ReturnsUnauthorized()
        {
            
            _optionMock.Setup(x => x.Value)
                .Returns(new ApiKeyOptions
                {
                    ValidApiKeys = new Dictionary<string, string>
                    {
                        { "Client", "ABC-123" }
                    }
                });
            
            _executingContext.HttpContext.Request.Headers["X-API-KEY"] = "INVALID-KEY";
            
            await _filter.OnActionExecutionAsync(_executingContext, _next);
            
            
            Assert.IsType<UnauthorizedObjectResult>(_executingContext.Result);
            Assert.Equal("Invalid API Key", 
                ((UnauthorizedObjectResult)_executingContext.Result).Value);
        }

        [Fact]
        public async Task OnActionExecutionAsync_ValidApiKey_ShouldContinuePipeline()
        {
            
            const string validKey = "ABC-123";
            
            _optionMock.Setup(x => x.Value)
                .Returns(new ApiKeyOptions
                {
                    ValidApiKeys = new Dictionary<string, string>
                    {
                        { "Client", validKey }
                    }
                });
            
            _executingContext.HttpContext.Request.Headers["X-API-KEY"] = validKey;
            
            await _filter.OnActionExecutionAsync(_executingContext, _next);
            
            Assert.Null(_executingContext.Result);
        }

        
        [Fact]
        public void ApiKeyAttribute_ShouldUseApiKeyFilter()
        {
            var attribute = new ApiKeyAttribute();
            
            Assert.Equal(typeof(ApiKeyFilter), attribute.ImplementationType);
        }
        
        [Fact]
        public async Task AllowAnonymousAttribute_ShouldSkipValidation()
        {
            _executingContext.ActionDescriptor.EndpointMetadata = 
                new List<object> { new AllowAnonymousAttribute() };
            
            await _filter.OnActionExecutionAsync(_executingContext, _next);
            
            Assert.Null(_executingContext.Result);
        }
    }
}