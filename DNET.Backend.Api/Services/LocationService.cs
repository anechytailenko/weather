using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using DNET.Backend.Api.Services.Interfaces;

namespace DNET.Backend.Api.Services
{
    public class LocationService: ILocationService

    {
    private readonly WeatherAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly LocationServiceOptions _locationServiceOptions;
    private readonly ILogger<LocationService> _logger;

    public LocationService(WeatherAppDbContext context, IMapper mapper,IOptionsSnapshot<LocationServiceOptions> locationServiceSettings,ILogger<LocationService> logger)
    {
        _context = context;
        _mapper = mapper;
        _locationServiceOptions = locationServiceSettings.Value;
        _logger = logger;
    }


    public async Task<List<LocationDTO>> GetAllLocations()
    {
        _logger.LogInformation("Retrieving all locations records.");
        
        var locations = await _context.Location
            .Include(l => l.Weathers)
            .Include(l => l.AlertLocations)
            .ThenInclude(al => al.Alert)
            .ProjectTo<LocationDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} locations.", locations.Count);
        return locations;
    }


    public async Task<LocationDTO?> GetLocationById(int id)
    {
        _logger.LogInformation("Getting location by ID {Id}", id);
        
        var location = await _context.Location
            .Include(l => l.Weathers)
            .Include(l => l.AlertLocations)
            .ThenInclude(al => al.Alert)
            .Where(l => l.Id == id)
            .ProjectTo<LocationDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (location == null)
        {
            _logger.LogWarning("Location with ID {Id} not found.", id);
        }
        else
        {
            _logger.LogInformation("Location with ID {Id} retrieved successfully.", id);
        }

        return location;
    }

    public async Task<LocationDTO?> CreateLocation(CreateLocationDTO createLocationDto)
    {
        _logger.LogInformation("Creating new location.");

        var countOfRecords = await _context.Location.CountAsync();
        
        if (countOfRecords >= _locationServiceOptions.MaxLocations)
        {
            _logger.LogWarning("Cannot create new location. Maximum number of locations reached.");
            return null;
        }

        var location = _mapper.Map<LocationEntity>(createLocationDto);
        
        location.AlertLocations = new List<AlertLocationEntity>();

        foreach (var alertId in createLocationDto.AlertIds)
        {
            var alert = await _context.Alert.FindAsync(alertId);
            if (alert != null)
            {
                location.AlertLocations.Add(new AlertLocationEntity { Alert = alert });
            }
            else
            {
                _logger.LogWarning("Alert with ID {AlertId} not found during creating location.", alertId);
            }
        }

        _context.Location.Add(location);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New location created with ID {Id}.", location.Id);
        return _mapper.Map<LocationDTO>(location);
    }


    public async Task<LocationDTO?> UpdateEntirelyLocation(int id, CreateLocationDTO updateLocationDto)
    {
        _logger.LogInformation("Updating location by ID {Id}.", id);
        var location = await _context.Location
            .Include(l => l.AlertLocations)
            .ThenInclude(al => al.Alert)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location == null)
        {
            _logger.LogWarning("Location with ID {Id} not found during updating.", id);
            return null;
        }

        
        _mapper.Map(updateLocationDto, location);
        
        location.AlertLocations.Clear();
        
        foreach (var alertId in updateLocationDto.AlertIds)
        {
            var alert = await _context.Alert.FindAsync(alertId);
            if (alert != null)
            {
                location.AlertLocations.Add(new AlertLocationEntity { Alert = alert });
            }
            else
            {
                _logger.LogWarning("Alert with ID {AlertId} not found during location update.", alertId);
            }
        }

        
        await _context.SaveChangesAsync();
        _logger.LogInformation("Location with ID {Id} updated successfully.", id);
        
        return _mapper.Map<LocationDTO>(location);
    }


    public async Task<bool> DeleteLocationById(int id)
    {
        
        if (!_locationServiceOptions.EnableDelete)
        {
            _logger.LogWarning("Delete operation is disabled in configuration.");
            return false;
        }
        
        _logger.LogInformation("Deleting location with ID {Id}.", id);
        var location = await _context.Location.FindAsync(id);

        if (location == null)
        {
            _logger.LogWarning("Location with ID {Id} was not found during deletion.", id);
            return false;
        }

        _context.Location.Remove(location);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Location with ID {Id} deleted successfully.", id);
        return true;
    }
    }
}