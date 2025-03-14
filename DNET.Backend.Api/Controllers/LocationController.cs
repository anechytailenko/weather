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
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        
        private readonly LocationServiceSettings _locationServiceSettings;
        
        public LocationController(ILocationService locationService, IOptionsSnapshot<LocationServiceSettings> locationServiceSettings)
        {
            _locationService = locationService;
            _locationServiceSettings = locationServiceSettings.Value;
        }
        
        
        [HttpPost]
        public IActionResult CreateNewLocation(Location location)
        {
            var newLocation = _locationService.Create(location);

            if (newLocation == null)
            {
                return Conflict("Cannot create a new location. The maximum number of locations has been reached.");
            }

            return Created($"/location/{newLocation.Id}", newLocation);
        }
        
        
        [HttpGet]
        public IActionResult GetAllLocations()
        {
            return Ok(_locationService.GetAllLocations());
        }

        
        [HttpGet("{id}")]
        public IActionResult GetLocationById(int id)
        {
            var location = _locationService.GetLocationById(id);
            return location != null ? Ok(location) : NotFound();
        }

        
        [HttpPut("{id}")]
        public IActionResult UpdateEntirelyLocationById(int id, Location location)
        {
            return _locationService.UpdateEntirelyLocation(id, location) != null ? Ok(location) : NotFound();
        }
        
        
        [HttpDelete("{id}")]
        public IActionResult DeleteLocationById(int id)
        {
            if (!_locationServiceSettings.EnableDelete)
            {
                return Conflict("Deletion is disabled due to configuration settings.");
            }
            return _locationService.DeleteLLocationById(id) ? NoContent() : NotFound();
        }

        
    }
}