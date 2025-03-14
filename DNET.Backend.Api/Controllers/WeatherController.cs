using System.Text.Json;
using DNET.Backend.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Services;
using Services.Interfaces;

namespace Controllers;

[ApiController]
[Route("/weather")]
public class WeatherController : ControllerBase
{
    private IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public IActionResult GetWeather()
    {
        var query = HttpContext.Request.Query;
    
        int.TryParse(query["limit"], out var limit);
        int.TryParse(query["offset"], out var offset);

        var weatherData = _weatherService.GetWeather(limit, offset);
    
        return Ok(weatherData);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetWeatherById(int id)
    {
        var record = _weatherService.GetWeatherById(id);
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public IActionResult CreateWeather(Weather data)
    {
        var record = _weatherService.CreateWeather(data);
        return Created($"/weather/{record.Id}", data);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateWeather(int id, Weather data)
    {
        var record = _weatherService.UpdateWeather(data);
        return record == null ? NotFound() : Ok(data);
    }

    [HttpPatch("{id:int}")]
    public IActionResult UpdateWeather(int id, JsonElement patch)
    {
        var record = _weatherService.UpdateWeather(id, patch);
        return record == null ? NotFound() : Ok(record);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteWeather(int id)
    {
        var record = _weatherService.DeleteWeather(id);
        return record == null ? NotFound() : NoContent();
    }
}