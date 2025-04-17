using System.Text.Json;
using DNET.Backend.Api.DTOs;

namespace DNET.Backend.Api.Services.Interfaces
{

    public interface IWeatherService
    {
        Task<List<WeatherDTO>> GetWeather(int limit, int offset);
        Task<WeatherDTO?> GetWeatherById(int id);
        Task<WeatherDTO> CreateWeather(CreateWeatherDTO weatherDto);

        Task<WeatherDTO?> UpdateWeather(int id, CreateWeatherDTO weatherDto);

        Task<WeatherDTO?> UpdateWeather(int id, JsonElement patch);
        
        Task<bool> DeleteWeather(int id);

        Task<WeatherDTO> FetchAndStoreWeather(string location);
    }
}