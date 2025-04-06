using System.Diagnostics;
using DNET.Backend.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DNET.Backend.Api.Tests
{
    public class LoggingMiddlewareTests
    {
        private readonly Mock<ILogger<LoggingMiddleware>> _loggerMock;
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly LoggingMiddleware _middleware;
        private readonly DefaultHttpContext _httpContext;

        public LoggingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<LoggingMiddleware>>();
            _nextMock = new Mock<RequestDelegate>();
            _middleware = new LoggingMiddleware(_nextMock.Object, _loggerMock.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestStart()
        {
            
            _httpContext.Request.Method = "GET";
            _httpContext.Request.Path = "/api/test";
            
            await _middleware.InvokeAsync(_httpContext);
            
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Request: GET /api/test")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        
        [Fact]
        public async Task InvokeAsync_LogsResponseDetails()
        {
            
            _httpContext.Request.Method = "POST";
            _httpContext.Request.Path = "/api/data";
            _httpContext.Response.StatusCode = 200;

           
            await _middleware.InvokeAsync(_httpContext);

          
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => 
                        v.ToString().Contains("Response: POST /api/data responded 200") &&
                        v.ToString().Contains("ms")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
       
    }
}