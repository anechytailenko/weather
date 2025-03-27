using Microsoft.AspNetCore.Mvc;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DNET.Backend.Api.Controllers
{
    [ApiController]
    [Route("/alert")]
    public class AlertController : ControllerBase
    {
        
        private readonly IAlertService _alertService;
        
        private readonly AlertServiceOptions _alertServiceOptions;
        
        public AlertController(IAlertService alertService, IOptionsSnapshot<AlertServiceOptions> alertServiceSettings)
        {
            _alertService = alertService;
            _alertServiceOptions = alertServiceSettings.Value;
        }
        
        
        [HttpPost]
        public async Task<IActionResult> CreateNewAlert(CreateAlertDTO alertDto)
        {
            var newAlert = await _alertService.CreateAlert(alertDto);
            
            if (newAlert == null)
            {
                return Conflict("Cannot create a new alert. The maximum number of alerts has been reached.");
            }
            return Created($"/alert/{newAlert.Id}", newAlert);
        } 
        
        
        [HttpGet]
        public async Task<IActionResult> GetAllAlerts()
        {
            return Ok(await _alertService.GetAllAlert());
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlertById(int id)
        {
            var alert = await _alertService.GetAlertById(id);
            return alert != null ? Ok(alert) : NotFound();
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntirelyLocationById(int id, CreateAlertDTO alertDto)
        {
            var existingRecord = await _alertService.UpdateEntirelyAlert(id, alertDto);
            return existingRecord != null ? Ok(existingRecord) : NotFound();
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlertById(int id)
        {
            if (!_alertServiceOptions.EnableDelete)
            {
                return Conflict("Deletion is disabled due to configuration settings.");
            }
            
            return await _alertService.DeleteAlertById(id) ? NoContent() : NotFound();
        }
    }
}