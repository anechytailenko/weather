using AutoMapper;
using Moq;
using DNET.Backend.Api.Services;
using Microsoft.Extensions.Options;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Profiles;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Tests.Profiles
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            
            _mapper = Utils.Get();
        }
        

        
        [Fact]
        public void AlertEntity_To_AlertDTO_Mapping_IsValid()
        {
            
            var alertEntity = new AlertEntity
            {
                Id = 1,
                Message = "Test Alert",
                IssuedAt = DateTime.UtcNow,
                AlertLocations = new List<AlertLocationEntity>
                {
                    new AlertLocationEntity
                    {
                        Location = new LocationEntity
                        {
                            Id = 10,
                            City = "Test City",
                            Country = "Test Country"
                        }
                    }
                }
            };
            
            var alertDto = _mapper.Map<AlertDTO>(alertEntity);
            
            Assert.Equal(alertEntity.Id, alertDto.Id);
            Assert.Equal(alertEntity.Message, alertDto.Message);
            Assert.Equal(alertEntity.IssuedAt, alertDto.IssuedAt);
            Assert.Single(alertDto.Locations);
            Assert.Equal(10, alertDto.Locations[0].Id);
            Assert.Equal("Test City", alertDto.Locations[0].City);
            Assert.Equal("Test Country", alertDto.Locations[0].Country);
            Assert.Null(alertDto.Locations[0].Weathers);
            Assert.Null(alertDto.Locations[0].Alerts);
        }

        
        
        [Fact]
        public void CreateAlertDTO_To_AlertEntity_Mapping_IsValid()
        {
           
            var createAlertDto = new CreateAlertDTO
            {
                Message = "New Alert",
                IssuedAt = DateTime.UtcNow,
                LocationIds = new List<int> { 1, 2, 3 }
            };
            
            var alertEntity = _mapper.Map<AlertEntity>(createAlertDto);
            
            Assert.Equal(createAlertDto.Message, alertEntity.Message);
            Assert.Equal(createAlertDto.IssuedAt, alertEntity.IssuedAt);
            Assert.NotNull(alertEntity.AlertLocations);
        }

        
        [Fact]
        public void LocationEntity_To_LocationDTO_Mapping_IsValid()
        {
            
            var locationEntity = new LocationEntity
            {
                Id = 1,
                City = "Test City",
                Country = "Test Country",
                Weathers = new List<WeatherEntity>
                {
                    new WeatherEntity
                    {
                        Id = 100,
                        Temperature = 25.5,
                        Condition = "Sunny",
                        RecordedAt = DateTime.UtcNow
                    }
                },
                AlertLocations = new List<AlertLocationEntity>
                {
                    new AlertLocationEntity
                    {
                        Alert = new AlertEntity
                        {
                            Id = 200,
                            Message = "Test Alert",
                            IssuedAt = DateTime.UtcNow
                        }
                    }
                }
            };
            
            var locationDto = _mapper.Map<LocationDTO>(locationEntity);
            
            Assert.Equal(locationEntity.Id, locationDto.Id);
            Assert.Equal(locationEntity.City, locationDto.City);
            Assert.Equal(locationEntity.Country, locationDto.Country);
            Assert.Equal(100, locationDto.Weathers[0].Id);
            Assert.Equal(25.5, locationDto.Weathers[0].Temperature);
            Assert.Equal("Sunny", locationDto.Weathers[0].Condition);
            Assert.Null(locationDto.Weathers[0].Location);
            Assert.Equal(200, locationDto.Alerts[0].Id);
            Assert.Equal("Test Alert", locationDto.Alerts[0].Message);
            Assert.Null(locationDto.Alerts[0].Locations);
        }

        [Fact]
        public void CreateLocationDTO_To_LocationEntity_Mapping_IsValid()
        {
            var createLocationDto = new CreateLocationDTO
            {
                City = "New City",
                Country = "New Country",
                AlertIds = new List<int> { 1, 2, 3 }
            };
            
            var locationEntity = _mapper.Map<LocationEntity>(createLocationDto);
            
            Assert.Equal(createLocationDto.City, locationEntity.City);
            Assert.Equal(createLocationDto.Country, locationEntity.Country);
            Assert.NotNull(locationEntity.Weathers);
            Assert.NotNull(locationEntity.AlertLocations);
        }

        
        
        [Fact]
        public void WeatherEntity_To_WeatherDTO_Mapping_IsValid()
        {
           
            var weatherEntity = new WeatherEntity
            {
                Id = 1,
                Temperature = 22.5,
                Condition = "Cloudy",
                RecordedAt = DateTime.UtcNow,
                Location = new LocationEntity
                {
                    Id = 10,
                    City = "Weather City",
                    Country = "Weather Country"
                }
            };

           
            var weatherDto = _mapper.Map<WeatherDTO>(weatherEntity);

            
            Assert.Equal(weatherEntity.Id, weatherDto.Id);
            Assert.Equal(weatherEntity.Temperature, weatherDto.Temperature);
            Assert.Equal(weatherEntity.Condition, weatherDto.Condition);
            Assert.Equal(weatherEntity.RecordedAt, weatherDto.RecordedAt);
            Assert.NotNull(weatherDto.Location);
            Assert.Equal(10, weatherDto.Location.Id);
            Assert.Equal("Weather City", weatherDto.Location.City);
            Assert.Equal("Weather Country", weatherDto.Location.Country);
            Assert.Null(weatherDto.Location.Weathers);
            Assert.Null(weatherDto.Location.Alerts);
        }
        
        

        [Fact]
        public void WeatherEntity_To_WeatherDTO_Mapping_WithNullLocation_IsValid()
        {
           
            var weatherEntity = new WeatherEntity
            {
                Id = 1,
                Temperature = 22.5,
                Condition = "Cloudy",
                RecordedAt = DateTime.UtcNow,
                Location = null
            };
            
            var weatherDto = _mapper.Map<WeatherDTO>(weatherEntity);
            
            Assert.Null(weatherDto.Location);
        }

        
        
        [Fact]
        public void CreateWeatherDTO_To_WeatherEntity_Mapping_IsValid()
        {
            
            var createWeatherDto = new CreateWeatherDTO
            {
                LocationId = 1,
                Temperature = 20.0,
                Condition = "Rainy",
                RecordedAt = DateTime.UtcNow
            };
            
            var weatherEntity = _mapper.Map<WeatherEntity>(createWeatherDto);
            
            Assert.Equal(createWeatherDto.Temperature, weatherEntity.Temperature);
            Assert.Equal(createWeatherDto.Condition, weatherEntity.Condition);
            Assert.Equal(createWeatherDto.RecordedAt, weatherEntity.RecordedAt);
            Assert.Null(weatherEntity.Location);
        }
        
    }
}