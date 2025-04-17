using System.Text.Json;
using DNET.Backend.Api.Attributes;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize(Policy = "RequireUser")]
    public async Task<IActionResult> GetWeather([FromQuery] int limit, [FromQuery] int offset)
    {
        var weatherData = await _weatherService.GetWeather(limit, offset);
    
        return Ok(weatherData);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequireUser")]
    public async Task<IActionResult> GetWeatherById(int id)
    {
        var record = await _weatherService.GetWeatherById(id);
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> CreateWeather(CreateWeatherDTO weatherDto)
    {
        var record = await _weatherService.CreateWeather(weatherDto);
        return Created($"/weather/{record.Id}", record);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> UpdateWeather(int id, CreateWeatherDTO data)
    {
        var record = await _weatherService.UpdateWeather(id, data);
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "RequireAdmin")]
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
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> DeleteWeather(int id)
    {
        var record = await _weatherService.DeleteWeather(id);
        return record == null ? NotFound() : NoContent();
    }
    
    
    [HttpGet("fetch")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> FetchWeather([FromQuery] string location)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location parameter is required");
            }

            var weather = await _weatherService.FetchAndStoreWeather(location);
            return Ok(weather);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching weather: {ex.Message}");
        }
    }
}