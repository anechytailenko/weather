using DNET.Backend.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DNET.Backend.Api.Controllers
{
    [ApiController]
    [ApiKey]
    [Route("/location")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        
        private readonly LocationServiceOptions _locationServiceOptions;
        
        public LocationController(ILocationService locationService, IOptionsSnapshot<LocationServiceOptions> locationServiceSettings)
        {
            _locationService = locationService;
            _locationServiceOptions = locationServiceSettings.Value;
        }
        
        
        [HttpPost]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> CreateNewLocation(CreateLocationDTO locationDto)
        {
            var newLocation = await _locationService.CreateLocation(locationDto);

            if (newLocation == null)
            {
                return Conflict("Cannot create a new location. The maximum number of locations has been reached.");
            }

            return Created($"/location/{newLocation.Id}", newLocation);
        }
        
        
        [HttpGet]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> GetAllLocations()
        {
            return Ok(await _locationService.GetAllLocations());
        }

        
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> GetLocationById(int id)
        {
            var location = await _locationService.GetLocationById(id);
            return location != null ? Ok(location) : NotFound();
        }
        
        
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> UpdateEntirelyLocationById(int id, CreateLocationDTO locationDto)
        {
            var existingRecord = await _locationService.UpdateEntirelyLocation(id, locationDto);
            return existingRecord != null ? Ok(existingRecord) : NotFound();
        }
        
        
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> DeleteLocationById(int id)
        {
            if (!_locationServiceOptions.EnableDelete)
            {
                return Conflict("Deletion is disabled due to configuration settings.");
            }
            return await _locationService.DeleteLocationById(id) ? NoContent() : NotFound();
        }
        
    }
}