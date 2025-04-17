using System.Net.Http.Json;
using DNET.Backend.Api.Requests;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;

namespace DNET.Backend.Api.Clients
{
    public class ExternalWeatherApiClient : IExternalWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalWeatherApiClient> _logger;
        private readonly AsyncRetryPolicy<ExternalWeatherResponse> _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<ExternalWeatherResponse> _circuitBreakerPolicy;

        public ExternalWeatherApiClient(HttpClient httpClient, ILogger<ExternalWeatherApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            
            _retryPolicy = Policy<ExternalWeatherResponse>
                .Handle<HttpRequestException>()
                .OrResult(x => x == null)
                .WaitAndRetryAsync(3, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, delay, retryCount, context) => 
                    {
                        _logger.LogWarning(
                            $"Retry {retryCount} of {context.PolicyKey}, reason: {exception.Exception?.Message ?? exception.Result?.ToString()}");
                    });
            
            
            _circuitBreakerPolicy = Policy<ExternalWeatherResponse>
                .Handle<HttpRequestException>()
                .OrResult(x => x == null)
                .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                    onBreak: (exception, state, duration, context) => 
                    {
                        _logger.LogError($"Next attempt in {duration.TotalSeconds}");
                    },
                    onReset: (context) => 
                    {
                        _logger.LogInformation("Circuit reset");
                    },
                    onHalfOpen: () => 
                    {
                        _logger.LogInformation("Circuit half-open");
                    });
        }

        public async Task<ExternalWeatherResponse> GetWeatherDataAsync(string location)
        {
            var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
            
            return await policyWrap.ExecuteAsync(async () => 
            {
                _logger.LogInformation($"Fetching weather data for {location}");
                
                var response = await _httpClient.GetFromJsonAsync<ExternalWeatherResponse>(
                    $"/weather?location={Uri.EscapeDataString(location)}");
                
                if (response == null)
                {
                    _logger.LogError("Received null response from weather API");
                    throw new Exception("Weather API returned null response");
                }
                
                return response;
            });
        }
    }
}