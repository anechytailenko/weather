using Models;
using Services;
using DNET.Backend.Api.DB;
using DNET.Backend.Api.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace DNET.Backend.Api.Tests;

[Collection("Sequential")]
public class WeatherServiceTests
{
    private WeatherService _weatherService;

    public WeatherServiceTests()
    {
        var optionsMock = new Mock<IOptionsMonitor<WeatherServiceOptions>>();
        optionsMock.Setup(o => o.CurrentValue).Returns(new WeatherServiceOptions
        {
            
            DefaultPaginationOffset = 0,
            DefaultPaginationLimit = 10,
            DefaultTemperatureUnit = "Celsius",
        });
        
        _weatherService = new WeatherService(optionsMock.Object);
        Db.weatherData.Clear();
        Insert2TestRecords();
    }

    private void Insert2TestRecords()
    {
        var newRecord1 = new Weather
            { Id = 1, LocationId = 1, Temperature = 20.5, Condition = "sunny", RecordedAt = new DateTime() };
        var newRecord2 = new Weather
            { Id = 2, LocationId = 2, Temperature = 18.5, Condition = "cloudy", RecordedAt = new DateTime() };
        
        _weatherService.CreateWeather(newRecord1);
        _weatherService.CreateWeather(newRecord2);
    }
    
    [Fact]
    public void GetWeather_ShouldReturnAllWeatherData()
    {
        var records = _weatherService.GetWeather();
        
        Assert.NotNull(records);
        Assert.Equal(2, records.Count);
    }
    
    [Fact]
    public void GetWeatherById_ShouldReturnWeatherData_WhenIdExists()
    {
        var result = _weatherService.GetWeatherById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void GetWeatherById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var result = _weatherService.GetWeatherById(9999);

        Assert.Null(result);
    }
    
    [Fact]
    public void CreateWeather_ShouldAddNewWeatherData()
    {
        var newRecord = new Weather
        {
            Id = 3,
            LocationId = 3,
            Temperature = 25.5,
            Condition = "rainy",
            RecordedAt = new DateTime()
        };

        var record = _weatherService.CreateWeather(newRecord);
        
        Assert.NotNull(record);
        Assert.Equal(newRecord.LocationId, record.LocationId);
        Assert.Equal(newRecord.Temperature, record.Temperature);
        Assert.Equal(newRecord.Condition, record.Condition);
    }
    
    [Fact]
    public void UpdateWeather_ShouldUpdateWeatherData_WhenIdExists()
    {
        var updatedRecord = new Weather
        {
            Id = 1,
            LocationId = 1,
            Temperature = 22.5,
            Condition = "cloudy",
            RecordedAt = new DateTime()
        };

        var result = _weatherService.UpdateWeather(updatedRecord);

        Assert.NotNull(result);
        Assert.Equal(22.5, result.Temperature);
        Assert.Equal("cloudy", result.Condition);
    }
    
    [Fact]
    public void UpdateWeather_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var updatedRecord = new Weather
        {
            Id = 9999,
            LocationId = 1,
            Temperature = 22.5,
            Condition = "cloudy",
            RecordedAt = new DateTime()
        };

        var result = _weatherService.UpdateWeather(updatedRecord);
        Assert.Null(result);
    }
    
    [Fact]
    public void DeleteWeather_ShouldRemoveWeatherData_WhenIdExists()
    {
        var record = _weatherService.GetWeatherById(1);
        var deletedRecord = _weatherService.DeleteWeather(1);
        
        Assert.NotNull(deletedRecord);
        Assert.Equal(record.Id, deletedRecord.Id);
        
        record = _weatherService.GetWeatherById(1);
        Assert.Null(record);
    }

    [Fact]
    public void DeleteWeather_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var deletedRecord = _weatherService.DeleteWeather(9999);
        Assert.Null(deletedRecord);
    }
}