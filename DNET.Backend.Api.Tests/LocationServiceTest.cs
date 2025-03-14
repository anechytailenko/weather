using Moq;
using Xunit;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using DNET.Backend.Api.DB;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DNET.Backend.Api.Tests
{
    public class LocationServiceTests {
        private  LocationService _locationService;
        private readonly Mock<IOptionsSnapshot<LocationServiceSettings>> _mockLocationServiceSettings;

        public LocationServiceTests() 
        {
            _mockLocationServiceSettings = new Mock<IOptionsSnapshot<LocationServiceSettings>>();
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceSettings { MaxLocations = 50 , EnableDelete = false });
            
            _locationService = new LocationService(_mockLocationServiceSettings.Object);
        }

        [Fact]
        public void GetAllAlert_ShouldReturnAllAlerts()
        {
            var amountOfRecordBefore = _locationService.GetAllLocations().Count();

            var location1 = new Location("Mumbai", "India1");
            var location2 = new Location ("Mumbai", "India2");
            Db.LocationModel.Add(location1);
            Db.LocationModel.Add(location2);

        
            var amountOfRecordAfter = _locationService.GetAllLocations().Count();

           
            Assert.Equal(amountOfRecordBefore+2, amountOfRecordAfter);
        }

        [Fact]
        public void GetAlertById_ShouldReturnAlert_WhenExists()
        {
            var result = _locationService.GetLocationById(5);
            
            Assert.NotNull(result);
            Assert.Equal("Paris", result?.City);
        }

        [Fact]
        public void GetAlertById_ShouldReturnNull_WhenNotExists()
        {
            var result = _locationService.GetLocationById(99);
            
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNewAlert_WhenUnderMaxAlerts()
        {
            int amountOfRecord = Db.LocationModel.Count;
            
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceSettings { MaxLocations = amountOfRecord + 2 , EnableDelete = false });
            _locationService = new LocationService(_mockLocationServiceSettings.Object);
            
            var location = new Location("TestCity", "TestCountry");
            
            var result = _locationService.Create(location);
        
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMaxAlertsReached()
        {
            int amountOfRecord = Db.LocationModel.Count;
            
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceSettings { MaxLocations = amountOfRecord -1 , EnableDelete = false });
            _locationService = new LocationService(_mockLocationServiceSettings.Object);
            
            var location = new Location("TestCity", "TestCountry");
            
            var result = _locationService.Create(location);
            Assert.Null(result);
        }

        [Fact]
        public void UpdateEntirelyAlert_ShouldReturnUpdatedAlert_WhenExists()
        {
            var updatedLocation = new Location("TestCity", "TestCountry");
           
            var result = _locationService.UpdateEntirelyLocation(3, updatedLocation);
            
            Assert.NotNull(result);
            Assert.Equal("TestCity", result?.City);
        }

        [Fact]
        public void UpdateEntirelyAlert_ShouldReturnNull_WhenNotExists()
        {
            var updatedLocation = new Location("TestCity", "TestCountry");
            var result = _locationService.UpdateEntirelyLocation(99, updatedLocation);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteLAlertById_ShouldReturnTrue_WhenAlertExists()
        {
            int amountOfRecord = Db.LocationModel.Count;
            
            _mockLocationServiceSettings.Setup(s => s.Value).Returns(new LocationServiceSettings { MaxLocations = amountOfRecord + 2 , EnableDelete = true });
          
            var location = new Location("TestCity", "TestCountry");
            var createdLocation = _locationService.Create(location);
            
            int idOfLastRecord =  createdLocation.Id;
            var result = _locationService.DeleteLLocationById(idOfLastRecord);
            
            Assert.True(result);
            Assert.Null(Db.LocationModel.Find(a => a.Id == idOfLastRecord));  
        }

        [Fact]
        public void DeleteLAlertById_ShouldReturnFalse_WhenAlertNotExists()
        {
           
            var result = _locationService.DeleteLLocationById(99);
            
            Assert.False(result);
        }
    }
}
