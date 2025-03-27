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

    public AlertService(WeatherAppDbContext context, IMapper mapper, IOptionsSnapshot<AlertServiceOptions> alertServiceSettings)
    {
        _context = context;
        _mapper = mapper;
        _alertServiceOptions = alertServiceSettings.Value;
        
    }

    
    public async Task<List<AlertDTO>> GetAllAlert()
    {
        
        return await _context.Alert
            .Include(a => a.AlertLocations)
            .ThenInclude(al => al.Location)
            .ProjectTo<AlertDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

   
    public async Task<AlertDTO?> GetAlertById(int id)
    {
        return await _context.Alert
            .Include(a => a.AlertLocations)
            .ThenInclude(al => al.Location)
            .Where(a => a.Id == id)
            .ProjectTo<AlertDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    
    public async Task<AlertDTO?> CreateAlert(CreateAlertDTO createAlertDto)
    {
        var countOfRecords = await _context.Alert.CountAsync();
        if (countOfRecords >= _alertServiceOptions.MaxAlerts)
        {
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
            }
        }
        
        _context.Alert.Add(alert);
        await _context.SaveChangesAsync();

     
        return _mapper.Map<AlertDTO>(alert);
    }
    
    
    public async Task<AlertDTO?> UpdateEntirelyAlert(int id, CreateAlertDTO updateAlertDto)
    {
        
        var alert = await _context.Alert
            .Include(a => a.AlertLocations)
            .ThenInclude(al => al.Location)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alert == null)
        {
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
        }
        
        await _context.SaveChangesAsync();
        
        return _mapper.Map<AlertDTO>(alert);
    }
    
    
    public async Task<bool> DeleteAlertById(int id)
    {
        if (_alertServiceOptions.EnableDelete == false) return false;
        var alert = await _context.Alert.FindAsync(id);

        if (alert == null)
        {
            return false;
        }

        _context.Alert.Remove(alert);
        await _context.SaveChangesAsync();

        return true;
    }
}
}
