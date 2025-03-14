using Models;
using System.Text.Json;
using DNET.Backend.Api.DB;
using DNET.Backend.Api.Options;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace Services;

public class WeatherService : IWeatherService
{
    private int lastWeatherId = 0;
    private IOptionsMonitor<WeatherServiceOptions> _options;
    public WeatherService(IOptionsMonitor<WeatherServiceOptions> options)
    {
        _options = options;
    }
    public List<Weather> GetWeather(int limit=0, int offset=0)
    {
        var options = _options.CurrentValue;
        if (limit == 0) limit = options.DefaultPaginationLimit;
        if (offset == 0) offset = options.DefaultPaginationOffset;
        return Db.weatherData.Skip(offset).Take(limit).ToList();
    }

    public Weather? GetWeatherById(int id)
    {
        return Db.weatherData.Find(x => x.Id == id);
    }

    public Weather CreateWeather(Weather weather)
    {
        lastWeatherId++;
        var newId = lastWeatherId;
        weather.Id = newId;
        Db.weatherData.Add(weather);
        return GetWeatherById(weather.Id);
    }

    public Weather? UpdateWeather(Weather weather)
    {
        var record = GetWeatherById(weather.Id);
        if (record == null) return null;
        
        record.LocationId = weather.LocationId;
        record.Temperature = weather.Temperature;
        record.Condition = weather.Condition;
        record.RecordedAt = weather.RecordedAt;
        
        return record;
    }

    public Weather? UpdateWeather(int id, JsonElement patch)
    {
        var record = GetWeatherById(id);
        if (record == null) return null;
    
        foreach (var field in patch.EnumerateObject())
        {
            switch (field.Name)
            {
                case "locationId":
                    record.LocationId = field.Value.GetInt32();
                    break;
                case "temperature":
                    record.Temperature = field.Value.GetDouble();
                    break;
                case "condition":
                    record.Condition = field.Value.GetString();
                    break;
                case "recordedAt":
                    record.RecordedAt = field.Value.GetDateTime();
                    break;
            }
        }
    
        return record;
    }

    public Weather? DeleteWeather(int id)
    {
        var record = GetWeatherById(id);
        if (record == null) return null;

        Db.weatherData.Remove(record);
        return record;
    }
}