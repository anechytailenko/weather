using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Models;

namespace DNET.Backend.Api.Tests;

[Collection("Sequential")]
public sealed class WeatherApiTests : BaseApiTests
{
    public WeatherApiTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GetWeather_ShouldReturnAllWeather()
    {
        var response = await Client.GetAsync("/weather");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var records = await response.Content.ReadFromJsonAsync<List<Weather>>();
        Assert.NotNull(records);
    }

    [Fact]
    public async Task GetWeatherById_ShouldReturnWeather_WhenExists()
    {
        var newRecord = new Weather()
            { Id = 1, LocationId = 1, Condition = "sunny", Temperature = 22.0, RecordedAt = new DateTime() };
        
        var response = await Client.PostAsJsonAsync("/weather", newRecord);
        var location = response.Headers.Location;
        
        response = await Client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var weatherData = await response.Content.ReadFromJsonAsync<Weather>();
        Assert.NotNull(weatherData);
        Assert.Equal("sunny", weatherData.Condition);
        
        await Client.DeleteAsync(location);
    }

    [Fact]
    public async Task GetWeatherById_ShouldReturnNotFound_WhenWeatherDoesNotExist()
    {
        var response = await Client.GetAsync("/weather/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateWeather_ShouldReturnCreated()
    {
        var newRecord = new Weather() { Id = 1, LocationId = 6, Condition = "rainy", Temperature = 6.1, RecordedAt = new DateTime() };

        var response = await Client.PostAsJsonAsync("/weather", newRecord);
        var location = response.Headers.Location;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var weatherData = await response.Content.ReadFromJsonAsync<Weather>();
        Assert.NotNull(weatherData);
        Assert.Equal(6, weatherData.LocationId);
        
        await Client.DeleteAsync(location);
    }
    
    [Fact]
    public async Task DeleteWeather_ShouldReturnNoContent_WhenWeatherExists()
    {
        var newRecord = new Weather() { Id = 1, LocationId = 12, Condition = "cloudy", Temperature = 12.1, RecordedAt = new DateTime() };
        
        var response = await Client.PostAsJsonAsync("/weather", newRecord);
        var location = response.Headers.Location;
        
        response = await Client.DeleteAsync(location);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteWeather_ShouldReturnNotFound_WhenWeatherDoesNotExist()
    {
        var response = await Client.DeleteAsync("/weather/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutWeather_ShouldReturnUpdatedWeather_WhenWeatherExists()
    {
        var newRecord = new Weather()
            { Id = 1, LocationId = 12, Condition = "windy", Temperature = -5.0, RecordedAt = new DateTime() };
        
        var response = await Client.PostAsJsonAsync("/weather", newRecord);
        var location = response.Headers.Location;
        
        var updatedRecord = new Weather()
            { Id = 1, LocationId = 12, Condition = "snowy", Temperature = -3.0, RecordedAt = new DateTime() };
        
        response = await Client.PutAsJsonAsync(location, updatedRecord);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await Client.GetAsync(location);
        var weatherData = await response.Content.ReadFromJsonAsync<Weather>();
        
        Assert.Equal("snowy", weatherData.Condition);
        Assert.Equal(-3.0, weatherData.Temperature);
        
        await Client.DeleteAsync(location);
    }

    [Fact]
    public async Task PutWeather_ShouldReturnNotFound_WhenWeatherDoesNotExists()
    {
        var updatedRecord = new Weather()
            { Id = 1, LocationId = 12, Condition = "snowy", Temperature = -3.0, RecordedAt = new DateTime() };
        
        var response = await Client.PutAsJsonAsync("/weather/1", updatedRecord);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task PatchWeather_ShouldReturnUpdatedWeather_WhenWeatherExists()
    {
        var newRecord = new Weather()
            { Id = 1, LocationId = 12, Condition = "windy", Temperature = -5.0, RecordedAt = new DateTime() };

        var response = await Client.PostAsJsonAsync("/weather", newRecord);
        var location = response.Headers.Location;

        var updatedRecord = new
        {
            LocationId = 34,
            Temperature = 12.0
        };
        
        response = await Client.PatchAsJsonAsync(location, updatedRecord);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        response = await Client.GetAsync(location);
        var weatherData = await response.Content.ReadFromJsonAsync<Weather>();
        
        // needs to be changed
        Assert.Equal(12.0, weatherData.Temperature);
        Assert.Equal(34, weatherData.LocationId);
        
        // needs to stay the same
        Assert.Equal("windy", weatherData.Condition);
        
        await Client.DeleteAsync("/weather/1");
    }

    [Fact]
    public async Task PatchWeather_ShouldReturnNotFound_WhenWeatherDoesNotExists()
    {
        var updatedRecord = new Weather()
            { Id = 1, LocationId = 12, Condition = "snowy", Temperature = -3.0, RecordedAt = new DateTime() };
        
        var response = await Client.PatchAsJsonAsync("/weather/1", updatedRecord);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}