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

    public LocationService(WeatherAppDbContext context, IMapper mapper,IOptionsSnapshot<LocationServiceOptions> locationServiceSettings)
    {
        _context = context;
        _mapper = mapper;
        _locationServiceOptions = locationServiceSettings.Value;
    }


    public async Task<List<LocationDTO>> GetAllLocations()
    {
        return await _context.Location
            .Include(l => l.Weathers)
            .Include(l => l.AlertLocations)
            .ThenInclude(al => al.Alert)
            .ProjectTo<LocationDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }


    public async Task<LocationDTO?> GetLocationById(int id)
    {
        return await _context.Location
            .Include(l => l.Weathers)
            .Include(l => l.AlertLocations)
            .ThenInclude(al => al.Alert)
            .Where(l => l.Id == id)
            .ProjectTo<LocationDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<LocationDTO?> CreateLocation(CreateLocationDTO createLocationDto)
    {
        var countOfRecords = await _context.Location.CountAsync();
        if (countOfRecords >= _locationServiceOptions.MaxLocations)
        {
            return null; 
        }
        
        var location = _mapper.Map<LocationEntity>(createLocationDto);
        location.AlertLocations = new List<AlertLocationEntity>();

        
        foreach (var alertId in createLocationDto.AlertIds)
        {
            var alert = await _context.Alert.FindAsync(alertId);
            if (alert != null)
            {
                location.AlertLocations.Add(new AlertLocationEntity
                {
                    Alert = alert,
                });
            }
        }
        
        _context.Location.Add(location);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<LocationDTO>(location);
    }


    public async Task<LocationDTO?> UpdateEntirelyLocation(int id, CreateLocationDTO updateLocationDto)
    {
        
        var location = await _context.Location
            .Include(l => l.AlertLocations)
            .ThenInclude(al => al.Alert)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location == null) return null;

        
        _mapper.Map(updateLocationDto, location);
        
        location.AlertLocations.Clear();
        
        foreach (var alertId in updateLocationDto.AlertIds)
        {
            var alert = await _context.Alert.FindAsync(alertId);
            if (alert != null)
            {
                location.AlertLocations.Add(new AlertLocationEntity
                {
                    Alert = alert
                });
            }
        }

        
        await _context.SaveChangesAsync();

        
        return _mapper.Map<LocationDTO>(location);
    }


    public async Task<bool> DeleteLocationById(int id)
    {
        
        if (_locationServiceOptions.EnableDelete == false) return false;
        var location = await _context.Location.FindAsync(id);

        if (location == null)
        {
            return false;
        }

        _context.Location.Remove(location);
        await _context.SaveChangesAsync();

        return true;
    }
    }
}