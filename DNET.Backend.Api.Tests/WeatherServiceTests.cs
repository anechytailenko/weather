using AutoMapper;
using DNET.Backend.Api.Clients;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services;
using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace DNET.Backend.Api.Tests;

[Collection("Sequential")]
public class WeatherServiceTests: IAsyncLifetime
{
    private WeatherService _weatherService;
    private IMapper _mapper;
    private WeatherAppDbContext _context;
    private  Mock<IExternalWeatherApiClient> _mockApiClient;
    public async Task InitializeAsync()
    {
        var optionsMock = new Mock<IOptionsSnapshot<WeatherServiceOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new WeatherServiceOptions
        {
            DefaultPaginationOffset = 0,
            DefaultPaginationLimit = 10,
            DefaultTemperatureUnit = "Celsius",
        });
        
        _mapper = Utils.Get();
        _context = Utils.CreateInMemoryDatabaseContext();
        _mockApiClient = new Mock<IExternalWeatherApiClient>();
        _weatherService = new WeatherService(_context, _mapper, optionsMock.Object,_mockApiClient.Object);
        await Insert2TestRecords();
    }
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task Insert2TestRecords()
    {
        //dependency of foreign_key
        Mock<IOptionsSnapshot<LocationServiceOptions>> _mockLocationServiceSettings = _mockLocationServiceSettings = new Mock<IOptionsSnapshot<LocationServiceOptions>>(); ;
        _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceOptions { MaxLocations = 50 , EnableDelete = false });
        LocationService _locationService = new LocationService(_context, _mapper, _mockLocationServiceSettings.Object);
        var location1 = new CreateLocationDTO {City  = "City 1", Country = "Country 1", AlertIds = new List<int>{2,3}};
        var createdLocation = await _locationService.CreateLocation(location1);
        Assert.Equal(1,createdLocation.Id);
        //dependency of foreign_key
        
        
        var newRecord1 = new CreateWeatherDTO{LocationId = 1, Temperature = 20.5, Condition = "sunny", RecordedAt = new DateTime()};
        var newRecord2 = new CreateWeatherDTO{LocationId = 2, Temperature = 20.5, Condition = "snowy", RecordedAt = new DateTime()};
        
        await _weatherService.CreateWeather(newRecord1);
        await _weatherService.CreateWeather(newRecord2);
    }
    
    [Fact]
    public async Task GetWeather_ShouldReturnAllWeatherData()
    {
        var amountOfRecords = await _context.Weather.CountAsync();
        var records = await _weatherService.GetWeather(-1,-1);
        
        Assert.NotNull(records);
        Assert.Equal(amountOfRecords, records.Count);
    }
    
    [Fact]
    public async Task GetWeatherById_ShouldReturnWeatherData_WhenIdExists()
    {
        var result = await _weatherService.GetWeatherById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetWeatherById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var result = await _weatherService.GetWeatherById(9999);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task CreateWeather_ShouldAddNewWeatherData()

    {
        
        var newRecord = new CreateWeatherDTO{LocationId = 1, Temperature = 20.5, Condition = "rainy", RecordedAt = new DateTime()};

        var record = await _weatherService.CreateWeather(newRecord);
        
        Assert.NotNull(record);
        Assert.Equal(newRecord.RecordedAt, record.RecordedAt);
        Assert.Equal(newRecord.Temperature, record.Temperature);
        Assert.Equal(newRecord.Condition, record.Condition);
    }
    
    [Fact]
    public async Task UpdateWeather_ShouldUpdateWeatherData_WhenIdExists()
    {
        var updatedRecord = new CreateWeatherDTO{LocationId = 1, Temperature = 22.5, Condition = "cloudy", RecordedAt = new DateTime()};

        var result = await _weatherService.UpdateWeather(1, updatedRecord);

        Assert.NotNull(result);
        Assert.Equal(22.5, result.Temperature);
        Assert.Equal("cloudy", result.Condition);
    }
    
    [Fact]
    public async Task UpdateWeather_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var updatedRecord = new CreateWeatherDTO{LocationId = 4, Temperature = 22.5, Condition = "fog", RecordedAt = new DateTime()};

        var result = await _weatherService.UpdateWeather(99, updatedRecord);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteWeather_ShouldRemoveWeatherData_WhenIdExists()
    {
        
        
        var newRecord = new CreateWeatherDTO{LocationId = 1, Temperature = 20.5, Condition = "rainy", RecordedAt = new DateTime()};
        var record = await _weatherService.CreateWeather(newRecord);
        Assert.NotNull(record);
        
        var deletedRecord = await _weatherService.DeleteWeather(record.Id);
        
        Assert.True(deletedRecord);
        
        record = await _weatherService.GetWeatherById(record.Id);
        Assert.Null(record);
    }

    [Fact]
    public async Task DeleteWeather_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var deletedRecord = await _weatherService.DeleteWeather(9999);
        Assert.False(deletedRecord);
    }
}