using System.Text.Json;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.DTOs;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Options;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using DNET.Backend.Api.Clients;
using DNET.Backend.Api.Services.Interfaces;



namespace DNET.Backend.Api.Services
{
    public class WeatherService: IWeatherService
    {
        private readonly WeatherAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly WeatherServiceOptions _weatherServiceOptions;
        private readonly IExternalWeatherApiClient _weatherApiClient;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(WeatherAppDbContext context, IMapper mapper,IOptionsSnapshot<WeatherServiceOptions> weatherServiceSettings,IExternalWeatherApiClient weatherApiClient,ILogger<WeatherService> logger)
        {
            _context = context;
            _mapper = mapper;
            _weatherServiceOptions = weatherServiceSettings.Value;
            _weatherApiClient = weatherApiClient;
            _logger = logger;
        }


        public async Task<List<WeatherDTO>> GetWeather(int limit, int offset)
        {

            _logger.LogDebug("Retrieve weather data with limit {Limit} and offset {Offset}", limit, offset);
            
            if (limit <= 0)
            {
                limit = _weatherServiceOptions.DefaultPaginationLimit;
                _logger.LogDebug("Limit adjusted to default value: {DefaultLimit}", limit);
            }

            if (offset < 0)
            {
                offset = _weatherServiceOptions.DefaultPaginationOffset;

                _logger.LogDebug("Offset adjusted to default value: {DefaultOffset}", offset);
            }
            
            try
            {
                var result = await _context.Weather
                    .Include(w => w.Location)
                    .Skip(offset)
                    .Take(limit)
                    .ProjectTo<WeatherDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} weather records", result.Count);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during retrieving weather data");
                throw;
            }
        }

        public async Task<WeatherDTO?> GetWeatherById(int id)
        {
            _logger.LogDebug("Retrieving weather data by ID: {WeatherId}", id);
            
            try
            {
                var weather = await _context.Weather
                    .Include(w => w.Location)
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (weather == null)
                {
                    _logger.LogWarning("Weather record with ID {WeatherId} not found", id);
                    return null;
                }
                
                _logger.LogDebug("Successfully retrieved weather record with ID {WeatherId}", id);
                return _mapper.Map<WeatherDTO>(weather);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while getting weather data for ID {WeatherId}", id);
                throw;
            }
        }


        public async Task<WeatherDTO> CreateWeather(CreateWeatherDTO createWeatherDto)
        {
            _logger.LogDebug("Creating new weather record");
            
            try
            {
                var location = await _context.Location.FindAsync(createWeatherDto.LocationId);
                
                if (location == null)
                {
                    _logger.LogWarning("Location with ID {LocationId} was not found", createWeatherDto.LocationId);
                    return null;
                }

                var weather = _mapper.Map<WeatherEntity>(createWeatherDto);
                weather.Location = location;

                _context.Weather.Add(weather);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created new weather record with ID {WeatherId}", weather.Id);
                return _mapper.Map<WeatherDTO>(weather);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating weather record");
                return null;
            }
        }

        
        
        public async Task<WeatherDTO?> UpdateWeather(int id, CreateWeatherDTO updateWeatherDto)
        {
            _logger.LogDebug("Updating weather record by ID {WeatherId}", id);
            
            try
            {
                var weather = await _context.Weather
                    .Include(w => w.Location)
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (weather == null)
                {
                    _logger.LogWarning("Weather record with ID {WeatherId} was not found during updating", id);
                    return null;
                }

                var location = await _context.Location.FindAsync(updateWeatherDto.LocationId);
                if (location == null)
                {
                    _logger.LogWarning("New location with ID {LocationId} was not found during updating", updateWeatherDto.LocationId);
                    return null;
                }

                _mapper.Map(updateWeatherDto, weather);
                weather.Location = location;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated weather record with ID {WeatherId}", id);
                return _mapper.Map<WeatherDTO>(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during updating weather record with ID {WeatherId}", id);
                return null;
            }
        }
        
        

        public async Task<WeatherDTO?> UpdateWeather(int id, JsonElement patch)
        {
            
            var weather = await _context.Weather
                .Include(w => w.Location)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (weather == null)
            {
                return null;
            }
            
            foreach (var field in patch.EnumerateObject())
            {
                switch (field.Name)
                {
                    case "locationId":
                        if (field.Value.ValueKind == JsonValueKind.Number)
                        {
                            var locationId = field.Value.GetInt32();
                            var location = await _context.Location.FindAsync(locationId);
                            if (location != null)
                            {
                                weather.LocationId = locationId;
                                weather.Location = location;
                            }
                        }
                        break;

                    case "temperature":
                        if (field.Value.ValueKind == JsonValueKind.Number)
                        {
                            weather.Temperature = field.Value.GetDouble();
                        }
                        break;

                    case "condition":
                        if (field.Value.ValueKind == JsonValueKind.String)
                        {
                            weather.Condition = field.Value.GetString();
                        }
                        break;

                    case "recordedAt":
                        if (field.Value.ValueKind == JsonValueKind.String && DateTime.TryParse(field.Value.GetString(), out var recordedAt))
                        {
                            weather.RecordedAt = recordedAt;
                        }
                        break;
                    default:
                        break;
                }
            }
            
            await _context.SaveChangesAsync();
            
            return _mapper.Map<WeatherDTO>(weather);
        }

  

        public async Task<bool> DeleteWeather(int id)
        {
            _logger.LogDebug("Deleting weather record by ID {WeatherId}", id);
            
            try
            {
                var weather = await _context.Weather.FindAsync(id);

                if (weather == null)
                {
                    _logger.LogWarning("Weather record with ID {WeatherId} was not found during deletion", id);
                    return false;
                }

                _context.Weather.Remove(weather);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully deleted weather record with ID {WeatherId}", id);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during deleting weather record by ID {WeatherId}", id);
                return false;
            }
        }
        
        
        public async Task<WeatherDTO> FetchAndStoreWeather(string location) {
            
            _logger.LogInformation("Fetching and storing weather data for location from external server: {Location}", location);
            
            try
            {
                var externalData = await _weatherApiClient.GetWeatherDataAsync(location);
                _logger.LogDebug("Successfully retrieved external weather data for {Location}", location);
        
                // assume LocationName in external api has format "City, Country"
                var locationComponents = externalData.LocationName.Split(',').Select(x => x.Trim()).ToArray();
        
                var city = locationComponents.Length > 0 ? locationComponents[0] : "Unknown";
                var country = locationComponents.Length > 1 ? locationComponents[1] : "Unknown";

                _logger.LogDebug("Processing weather data for {City}, {Country}", city, country);
               
                var locationEntity = await _context.Location.FirstOrDefaultAsync(l => l.City == city && l.Country == country);
                
                if (locationEntity == null)
                {
                    _logger.LogDebug("Creating new location entity for {City}, {Country}", city, country);
                    locationEntity = new LocationEntity 
                    { 
                        City = city,
                        Country = country,
                        Weathers = new List<WeatherEntity>()
                    };
                }

                var weatherEntity = new WeatherEntity
                {
                    Temperature = externalData.Temperature,
                    Condition = externalData.Condition,
                    RecordedAt = externalData.ObservationTime,
                    Location = locationEntity
                };

                locationEntity.Weathers.Add(weatherEntity);
        
                if (locationEntity.Id == 0)
                {
                    _logger.LogDebug("Adding new location to database");
                    _context.Location.Add(locationEntity);
                }
                
                _logger.LogDebug("Adding new weather record to database");
                _context.Weather.Add(weatherEntity);
                
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully stored weather data for {City}, {Country} with ID {WeatherId}", 
                    city, country, weatherEntity.Id);
                
                return _mapper.Map<WeatherDTO>(weatherEntity);
            }
            catch (Exception e) when (e is not ApplicationException)
            {
                _logger.LogError(e, "Failed to fetch and store weather data for location: {Location}", location);
                throw new ApplicationException("Failed to fetch weather data", e);
            }
        }  
    }
        
    
}