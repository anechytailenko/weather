using Microsoft.AspNetCore.Mvc;

using DNET.Backend.Api.DB;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Services;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.Extensions.Options;


namespace DNET.Backend.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        
        private readonly IAlertService _alertService;
        
        private readonly AlertServiceSettings _alertServiceSettings;
        
        public AlertController(IAlertService alertService, IOptionsSnapshot<AlertServiceSettings> alertServiceSettings)
        {
            _alertService = alertService;
            _alertServiceSettings = alertServiceSettings.Value;
        }
        
        
        [HttpPost]
        public IActionResult CreateNewAlert(Alert alert)
        {
            var newAlert = _alertService.Create(alert);
            
            if (newAlert == null)
            {
                return Conflict("Cannot create a new alert. The maximum number of alerts has been reached.");
            }
            return Created($"/alert/{newAlert.Id}", newAlert);
        } 
        
        
        [HttpGet]
        public IActionResult GetAllAlerts()
        {
            return Ok(_alertService.GetAllAlert());
        }

        
        [HttpGet("{id}")]
        public IActionResult GetAlertById(int id)
        {
            var alert = _alertService.GetAlertById(id);
            return alert != null ? Ok(alert) : NotFound();
        }

        
        [HttpPut("{id}")]
        public IActionResult UpdateEntirelyLocationById(int id, Alert alert)
        {
            return _alertService.UpdateEntirelyAlert(id, alert) != null ? Ok(alert) : NotFound();
        }
        

        [HttpDelete("{id}")]
        public IActionResult DeleteAlertById(int id)
        {
            if (!_alertServiceSettings.EnableDelete)
            {
                return Conflict("Deletion is disabled due to configuration settings.");
            }
            
            return _alertService.DeleteLAlertById(id) ? NoContent() : NotFound();
        }
    }
}