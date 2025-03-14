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
    public class AlertServiceTests {
        private  AlertService _alertService;
        private readonly Mock<IOptionsSnapshot<AlertServiceSettings>> _mockAlertServiceSettings;

        public AlertServiceTests() 
        {
            _mockAlertServiceSettings = new Mock<IOptionsSnapshot<AlertServiceSettings>>();
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceSettings { MaxAlerts = 50 , EnableDelete = false });
            
            _alertService = new AlertService(_mockAlertServiceSettings.Object);
        }

        [Fact]
        public void GetAllAlert_ShouldReturnAllAlerts()
        {
            var amountOfRecordBefore = _alertService.GetAllAlert().Count();
          
            var alert1 = new Alert ( 1, "Alert 1", DateTime.Now.AddHours(-2) );
            var alert2 = new Alert ( 1, "Alert 2", DateTime.Now.AddHours(-3) );
            Db.AlertModel.Add(alert1);
            Db.AlertModel.Add(alert2);

        
            var amountOfRecordAfter = _alertService.GetAllAlert().Count();

           
            Assert.Equal(amountOfRecordBefore+2, amountOfRecordAfter);
        }

        [Fact]
        public void GetAlertById_ShouldReturnAlert_WhenExists()
        {
            var result = _alertService.GetAlertById(1);
            
            Assert.NotNull(result);
            Assert.Equal("Fire hazard detected", result?.Message);
        }

        [Fact]
        public void GetAlertById_ShouldReturnNull_WhenNotExists()
        {
            var result = _alertService.GetAlertById(99);
            
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNewAlert_WhenUnderMaxAlerts()
        {
            int amountOfRecord = Db.AlertModel.Count;
            
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceSettings { MaxAlerts = amountOfRecord + 2 , EnableDelete = false });
            _alertService = new AlertService(_mockAlertServiceSettings.Object);
            
            var alert = new Alert( 1, "Alert 1", DateTime.Now.AddHours(-2) );
            
            var result = _alertService.Create(alert);
        
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMaxAlertsReached()
        {
            int amountOfRecord = Db.AlertModel.Count;
            
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceSettings { MaxAlerts = amountOfRecord -1 , EnableDelete = false });
            _alertService = new AlertService(_mockAlertServiceSettings.Object);
            
            var alert = new Alert( 1, "Alert 1", DateTime.Now.AddHours(-2) );
            
            var result = _alertService.Create(alert);
            Assert.Null(result);
        }

        [Fact]
        public void UpdateEntirelyAlert_ShouldReturnUpdatedAlert_WhenExists()
        {
            var updatedAlert = new Alert( 5, "Severe weather warning", DateTime.Now);
           
            var result = _alertService.UpdateEntirelyAlert(3, updatedAlert);
            
            Assert.NotNull(result);
            Assert.Equal(5, result?.LocationId);
        }

        [Fact]
        public void UpdateEntirelyAlert_ShouldReturnNull_WhenNotExists()
        {
            var updatedAlert = new Alert( 5, "Severe weather warning", DateTime.Now);
            var result = _alertService.UpdateEntirelyAlert(99, updatedAlert);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteLAlertById_ShouldReturnTrue_WhenAlertExists()
        {
            int amountOfRecord = Db.AlertModel.Count;
            
            _mockAlertServiceSettings.Setup(s => s.Value).Returns(new AlertServiceSettings { MaxAlerts = amountOfRecord + 2 , EnableDelete = true });
          
            var alert = new Alert ( 5, "Severe weather warning", DateTime.Now);
            var createdAlert = _alertService.Create(alert);
            
            int idOfLastRecord =  createdAlert.Id;
            var result = _alertService.DeleteLAlertById(idOfLastRecord);
            
            Assert.True(result);
            Assert.Null(Db.AlertModel.Find(a => a.Id == idOfLastRecord));  
        }

        [Fact]
        public void DeleteLAlertById_ShouldReturnFalse_WhenAlertNotExists()
        {
           
            var result = _alertService.DeleteLAlertById(99);
            
            Assert.False(result);
        }
    }
}
