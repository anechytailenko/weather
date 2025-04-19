using AutoMapper;
using Moq;
using DNET.Backend.Api.Services;
using Microsoft.Extensions.Options;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Profiles;
using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DNET.Backend.Api.Tests
{   
    [Collection("Sequential")]
    public class AlertServiceTests : IAsyncLifetime
    {
        
        private AlertService _alertService;
        private  Mock<IOptionsSnapshot<AlertServiceOptions>> _mockAlertServiceSettings;
        private IMapper _mapper;
        private WeatherAppDbContext _context;
        private Mock<ILogger<AlertService>> _mockLogger;

        public async Task InitializeAsync()
        {
            _mockAlertServiceSettings = new Mock<IOptionsSnapshot<AlertServiceOptions>>();
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceOptions { MaxAlerts = 50 , EnableDelete = false });
            _mapper = Utils.Get();
            _context = Utils.CreateInMemoryDatabaseContext();
            _mockLogger = new Mock<ILogger<AlertService>>();
            
            _alertService = new AlertService(_context,_mapper, _mockAlertServiceSettings.Object,_mockLogger.Object);

            await InsertMockData();
        }
        public Task DisposeAsync() => Task.CompletedTask; 

        public async Task InsertMockData()
        {
            var alert1 = new CreateAlertDTO {Message  = "Alert 1",IssuedAt = DateTime.Now.AddHours(-2) ,LocationIds = new List<int>{1}};
            var alert2 = new CreateAlertDTO {Message  = "Alert 2",IssuedAt = DateTime.Now.AddHours(-3) ,LocationIds = new List<int>{1}};
            
            await _alertService.CreateAlert(alert1);
            await _alertService.CreateAlert(alert2);
           
        }

        [Fact]
        public async Task GetAllAlert_ShouldReturnAllAlerts()
        {
            var amountOfRecordBefore = await _context.Alert.CountAsync();

            var alert3 = new CreateAlertDTO {Message  = "Alert 3",IssuedAt = DateTime.Now.AddHours(-4) ,LocationIds = new List<int>{}};
            
            var newRecord = await _alertService.CreateAlert(alert3);
            Assert.NotNull(newRecord);
            
            var amountOfRecordAfter = await _context.Alert.CountAsync();

           
            Assert.Equal(amountOfRecordBefore+1, amountOfRecordAfter);
        }

        [Fact]
        public async Task GetAlertById_ShouldReturnAlert_WhenExists()
        {
            int id = 1;
            var result = await _alertService.GetAlertById(id);
            
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetAlertById_ShouldReturnNull_WhenNotExists()
        {
            var result = await _alertService.GetAlertById(99);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNewAlert_WhenUnderMaxAlerts()
        {
            int amountOfRecord = await _context.Alert.CountAsync();
            
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceOptions { MaxAlerts = amountOfRecord + 2 , EnableDelete = false });
            _alertService = new AlertService(_context,_mapper, _mockAlertServiceSettings.Object,_mockLogger.Object);

            var alert = new CreateAlertDTO { Message = "Alert 4", IssuedAt = DateTime.Now.AddHours(-4), LocationIds = new List<int> { } };
            
            var result = await _alertService.CreateAlert(alert);
        
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenMaxAlertsReached()
        {
            
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceOptions { MaxAlerts = 0 , EnableDelete = false }); 
            _alertService = new AlertService(_context,_mapper, _mockAlertServiceSettings.Object,_mockLogger.Object);
            
            var alert = new CreateAlertDTO { Message = "Alert 5", IssuedAt = DateTime.Now.AddHours(-4), LocationIds = new List<int> {  } };
            
            var result = await _alertService.CreateAlert(alert);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateEntirelyAlert_ShouldReturnUpdatedAlert_WhenExists()
        {
            var updatedAlert = new CreateAlertDTO { Message = "Alert 10", IssuedAt = DateTime.Now.AddHours(-4), LocationIds = new List<int> { } };
           
            var result = await _alertService.UpdateEntirelyAlert(1, updatedAlert);
            
            Assert.NotNull(result);
            Assert.Equal( "Alert 10", updatedAlert.Message );
        }

        [Fact]
        public async Task UpdateEntirelyAlert_ShouldReturnNull_WhenNotExists()
        {
            var updatedAlert = new CreateAlertDTO { Message = "Alert 11", IssuedAt = DateTime.Now.AddHours(-4), LocationIds = new List<int> { } };
            var result = await _alertService.UpdateEntirelyAlert(99, updatedAlert);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteLAlertById_ShouldReturnTrue_WhenAlertExists()
        {
            int amountOfRecord = await _context.Alert.CountAsync();
            
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceOptions { MaxAlerts = amountOfRecord + 2 , EnableDelete = true });
            _alertService = new AlertService(_context,_mapper, _mockAlertServiceSettings.Object,_mockLogger.Object);
            var alert =new CreateAlertDTO { Message = "Alert 12", IssuedAt = DateTime.Now.AddHours(-4), LocationIds = new List<int> { } };
            var createdAlert = await _alertService.CreateAlert(alert);

            Assert.NotNull(createdAlert);
            var result = await _alertService.DeleteAlertById( createdAlert.Id);
            
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteLAlertById_ShouldReturnFalse_WhenAlertNotExists()
        {
            var result = await _alertService.DeleteAlertById(99);
            Assert.False(result);
        }
    }
}
