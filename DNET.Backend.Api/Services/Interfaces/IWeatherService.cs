using System.Text.Json;
using Models;

namespace Services.Interfaces;

public interface IWeatherService
{
    List<Weather> GetWeather(int limit, int offset);
    Weather? GetWeatherById(int id);
    Weather CreateWeather(Weather weather);
    Weather? UpdateWeather(Weather weather);
    Weather? UpdateWeather(int id, JsonElement patch);
    Weather? DeleteWeather(int id);
}