using System.Text.Json;
using DNET.Backend.Api.Attributes;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DNET.Backend.Api.Services.Interfaces;

namespace Controllers;

[ApiController]
[ApiKey]
[Route("/weather")]
public class WeatherController : ControllerBase
{
    private IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] int limit, [FromQuery] int offset)
    {
        var weatherData = await _weatherService.GetWeather(limit, offset);
    
        return Ok(weatherData);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWeatherById(int id)
    {
        var record = await _weatherService.GetWeatherById(id);
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWeather(CreateWeatherDTO weatherDto)
    {
        var record = await _weatherService.CreateWeather(weatherDto);
        return Created($"/weather/{record.Id}", record);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWeather(int id, CreateWeatherDTO data)
    {
        var record = await _weatherService.UpdateWeather(id, data);
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<WeatherDTO>> UpdateWeather(int id, [FromBody] JsonElement patch)
    {
        if (patch.ValueKind == JsonValueKind.Null || patch.ValueKind == JsonValueKind.Undefined)
        {
            return BadRequest("Patch document is required.");
        }

        var updatedWeather = await _weatherService.UpdateWeather(id, patch);

        if (updatedWeather == null)
        {
            return NotFound($"Weather record with ID {id} not found.");
        }

        return Ok(updatedWeather);
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWeather(int id)
    {
        var record = await _weatherService.DeleteWeather(id);
        return record == null ? NotFound() : NoContent();
    }
}