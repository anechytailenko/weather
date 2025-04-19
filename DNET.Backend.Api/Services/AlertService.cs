using DNET.Backend.Api.Options;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNET.Backend.Api.Services.Interfaces;

namespace DNET.Backend.Api.Services
{
    
public class AlertService: IAlertService
{
    private readonly WeatherAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly AlertServiceOptions _alertServiceOptions;
    private readonly ILogger<AlertService> _logger;

    public AlertService(WeatherAppDbContext context, IMapper mapper, IOptionsSnapshot<AlertServiceOptions> alertServiceSettings, ILogger<AlertService> logger)
    {
        _context = context;
        _mapper = mapper;
        _alertServiceOptions = alertServiceSettings.Value;
        _logger = logger;
    }

    
    public async Task<List<AlertDTO>> GetAllAlert()
    {
        _logger.LogInformation("Retrieving all alerts.");
        return await _context.Alert
            .Include(a => a.AlertLocations)
            .ThenInclude(al => al.Location)
            .ProjectTo<AlertDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

   
    public async Task<AlertDTO?> GetAlertById(int id)
    {
        _logger.LogInformation("Retrieving alert by ID: {AlertId}", id);
        return await _context.Alert
            .Include(a => a.AlertLocations)
            .ThenInclude(al => al.Location)
            .Where(a => a.Id == id)
            .ProjectTo<AlertDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    
    public async Task<AlertDTO?> CreateAlert(CreateAlertDTO createAlertDto)
    {
        _logger.LogInformation("Creating new alert.");
        
        var countOfRecords = await _context.Alert.CountAsync();
        if (countOfRecords >= _alertServiceOptions.MaxAlerts)
        {
            _logger.LogWarning("Alert creation was terminated. Max alerts limit ({MaxAlerts}) reached.", _alertServiceOptions.MaxAlerts);
            return null; 
        }
        
        
        var alert = _mapper.Map<AlertEntity>(createAlertDto);
        alert.AlertLocations = new List<AlertLocationEntity>();
        
       
        foreach (var locationId in createAlertDto.LocationIds)
        {
            var location = await _context.Location.FindAsync(locationId);
            if (location != null)
            {
                alert.AlertLocations.Add(new AlertLocationEntity
                {
                    Location = location,
                });
            }else
            {
                _logger.LogWarning("Location ID {LocationId} not found during alert creation.", locationId);
            }
        }
        
        _context.Alert.Add(alert);
        await _context.SaveChangesAsync();

     
        return _mapper.Map<AlertDTO>(alert);
    }
    
    
    public async Task<AlertDTO?> UpdateEntirelyAlert(int id, CreateAlertDTO updateAlertDto)
    {
        _logger.LogInformation("Updating alert with ID: {AlertId}", id);

        
        var alert = await _context.Alert
            .Include(a => a.AlertLocations)
            .ThenInclude(al => al.Location)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alert == null)
        {
            _logger.LogWarning("Alert with ID {AlertId} was not found during update.", id);
            return null;
        }

        
        _mapper.Map(updateAlertDto, alert);
        
        alert.AlertLocations.Clear();
        
        foreach (var locationId in updateAlertDto.LocationIds)
        {
            var location = await _context.Location.FindAsync(locationId);
            if (location != null)
            {
                alert.AlertLocations.Add(new AlertLocationEntity
                {
                    Location = location,
                });
            } 
            else
            {
                _logger.LogWarning("Location ID {LocationId} was not found during update.", locationId);
            }
        }
        
        await _context.SaveChangesAsync();
        
        return _mapper.Map<AlertDTO>(alert);
    }
    
    
    public async Task<bool> DeleteAlertById(int id)
    {
        _logger.LogInformation("Deleting alert by ID: {AlertId}", id);
        
        if (!_alertServiceOptions.EnableDelete)
        {
            _logger.LogWarning("Alert deletion is disabled by configuration.");
            return false;
        }
        
        var alert = await _context.Alert.FindAsync(id);

        if (alert == null)
        {
            _logger.LogWarning("Alert with ID {AlertId} was not found during deletion.", id);
            return false;
        }
        
        _logger.LogInformation("Alert with ID {AlertId} deleted successfully.", id);
        
        _context.Alert.Remove(alert);
        await _context.SaveChangesAsync();

        return true;
    }
}
}
