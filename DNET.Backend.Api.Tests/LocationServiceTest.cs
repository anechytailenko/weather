using AutoMapper;
using Moq;
using DNET.Backend.Api.Services;
using Microsoft.Extensions.Options;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DNET.Backend.Api.Tests
{   
    [Collection("Sequential")]
    public class LocationServiceTests: IAsyncLifetime {
        
        private LocationService _locationService;
        private  Mock<IOptionsSnapshot<LocationServiceOptions>> _mockLocationServiceSettings;
        private IMapper _mapper;
        private WeatherAppDbContext _context;
        private Mock<ILogger<LocationService>> _mockLogger;

        public async Task InitializeAsync()
        {
            _mockLocationServiceSettings = new Mock<IOptionsSnapshot<LocationServiceOptions>>();
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceOptions { MaxLocations = 50 , EnableDelete = false });
            _mapper = Utils.Get();
            _context = Utils.CreateInMemoryDatabaseContext();
            _mockLogger = new Mock<ILogger<LocationService>>();
            _locationService = new LocationService(_context, _mapper, _mockLocationServiceSettings.Object,_mockLogger.Object);

            await InsertMockData();
        }
        public Task DisposeAsync() => Task.CompletedTask; 

        public async Task InsertMockData()
        {
            var location1 = new CreateLocationDTO {City  = "City 1", Country = "Country 1", AlertIds = new List<int>{1,2}};
            var location2 = new CreateLocationDTO {City  = "City 1", Country = "Country 1", AlertIds = new List<int>{2,3}};
            
            await _locationService.CreateLocation(location1);
            await _locationService.CreateLocation(location2);
        }

        [Fact]
        public async Task GetAllLocation_ShouldReturnAllLocations()
        {
            var amountOfRecordBefore = await _context.Location.CountAsync();

            var location3 = new CreateLocationDTO {City  = "City 3", Country = "Country 3", AlertIds = new List<int>{}};
            var createLocation =  _locationService.CreateLocation(location3);

            Assert.NotNull(createLocation);
            var amountOfRecordAfter = await _context.Location.CountAsync();

           
            Assert.Equal(amountOfRecordBefore+1, amountOfRecordAfter);
        }

        [Fact]
        public async Task GetLocationById_ShouldReturnLocation_WhenExists()
        {
            int id = 1;
            var result = await _locationService.GetLocationById(id);
            
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetLocationById_ShouldReturnNull_WhenNotExists()
        {
            var result = await _locationService.GetLocationById(99);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNewLocation_WhenUnderMaxLocations()
        {
            int amountOfRecord = await _context.Location.CountAsync();
            
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceOptions { MaxLocations = amountOfRecord + 2 , EnableDelete = false });
            _locationService = new LocationService(Utils.CreateInMemoryDatabaseContext(),_mapper, _mockLocationServiceSettings.Object,_mockLogger.Object);
            
            var location = new CreateLocationDTO {City  = "City 4", Country = "Country 4", AlertIds = new List<int>{1,4}};
            
            var result = await _locationService.CreateLocation(location);
        
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenMaxLocationsReached()
        {
            int amountOfRecord = await _context.Location.CountAsync();
            
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceOptions { MaxLocations = 0 , EnableDelete = false }); 
            _locationService = new LocationService(Utils.CreateInMemoryDatabaseContext(),_mapper, _mockLocationServiceSettings.Object,_mockLogger.Object);
            
            var location = new CreateLocationDTO{City  = "City 5", Country = "Country 5", AlertIds = new List<int>{4,5}};
            
            var result = await _locationService.CreateLocation(location);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateEntirelyLocation_ShouldReturnUpdatedLocation_WhenExists()
        {
            var updatedLocation = new CreateLocationDTO{City  = "City 6", Country = "Country 6", AlertIds = new List<int>{1,5}};
           
            var result = await _locationService.UpdateEntirelyLocation(1, updatedLocation);
            
            Assert.NotNull(result);
            Assert.Equal("City 6", result.City);
        }

        [Fact]
        public async Task UpdateEntirelyLocation_ShouldReturnNull_WhenNotExists()
        {
            var updatedLocation = new CreateLocationDTO{City  = "City 7", Country = "Country 7", AlertIds = new List<int>{5}};
            var result = await _locationService.UpdateEntirelyLocation(99, updatedLocation);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteLocationById_ShouldReturnTrue_WhenLocationExists()
        {
            int amountOfRecord = (await _locationService.GetAllLocations()).Count();
            
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceOptions { MaxLocations = amountOfRecord + 2 , EnableDelete = true });
            _locationService = new LocationService(Utils.CreateInMemoryDatabaseContext(),_mapper, _mockLocationServiceSettings.Object,_mockLogger.Object);
            var location = new CreateLocationDTO{City  = "City 8", Country = "Country 8", AlertIds = new List<int>{2,6}};
            var createdAlert = await _locationService.CreateLocation(location);
            
            int idOfLastRecord = createdAlert.Id;
            var result = await _locationService.DeleteLocationById(idOfLastRecord);
            
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteLocationById_ShouldReturnFalse_WhenLocationNotExists()
        {
           
            var result = await _locationService.DeleteLocationById(99);
            
            Assert.False(result);
        }
    }
}
