using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Services.Interfaces;

namespace DNET.Backend.Api.Services
{
    public class WeatherServiceWithCache : IWeatherService
    {
        private readonly IWeatherService _inner;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherServiceWithCache> _logger;

        public WeatherServiceWithCache(IWeatherService inner, IMemoryCache cache, ILogger<WeatherServiceWithCache> logger)
        {
            _inner = inner;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeatherDTO?> GetWeatherById(int id)
        {
            var cacheKey = $"weather_{id}";

            if (_cache.TryGetValue(cacheKey, out WeatherDTO cachedWeather))
            {
                _logger.LogInformation("Returning weather data for id {Id} from cache", id);
                return cachedWeather;
            }

            var weather = await _inner.GetWeatherById(id);
            if (weather != null)
            {
                _cache.Set(cacheKey, weather, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Caching weather data for id {Id}", id);
            }

            return weather;
        }

  

        public Task<List<WeatherDTO>> GetWeather(int limit, int offset) => _inner.GetWeather(limit, offset);

        public Task<WeatherDTO> CreateWeather(CreateWeatherDTO createWeatherDto) => _inner.CreateWeather(createWeatherDto);

        public Task<WeatherDTO?> UpdateWeather(int id, CreateWeatherDTO updateWeatherDto) => _inner.UpdateWeather(id, updateWeatherDto);

        public Task<WeatherDTO?> UpdateWeather(int id, JsonElement patch) => _inner.UpdateWeather(id, patch);

        public Task<bool> DeleteWeather(int id) => _inner.DeleteWeather(id);
        public Task<WeatherDTO> FetchAndStoreWeather(string location)
        {
            throw new NotImplementedException();
        }
    }
}