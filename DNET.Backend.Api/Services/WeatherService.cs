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

using DNET.Backend.Api.Services.Interfaces;



namespace DNET.Backend.Api.Services
{
    public class WeatherService: IWeatherService
    {
        private readonly WeatherAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly WeatherServiceOptions _weatherServiceOptions;

        public WeatherService(WeatherAppDbContext context, IMapper mapper,IOptionsSnapshot<WeatherServiceOptions> weatherServiceSettings)
        {
            _context = context;
            _mapper = mapper;
            _weatherServiceOptions = weatherServiceSettings.Value;
        }


        public async Task<List<WeatherDTO>> GetWeather(int limit, int offset)
        {
            if (limit <= 0)
            {
                limit = _weatherServiceOptions.DefaultPaginationLimit;
            }

            if (offset < 0)
            {
                offset = _weatherServiceOptions.DefaultPaginationOffset;
            }
            
            return await _context.Weather
                .Include(w => w.Location)
                .Skip(offset)
                .Take(limit)
                .ProjectTo<WeatherDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<WeatherDTO?> GetWeatherById(int id)
        {
            var weather = await _context.Weather
                .Include(w => w.Location)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (weather == null)
            {
                return null;
            }
            
            return _mapper.Map<WeatherDTO>(weather);
        }


        public async Task<WeatherDTO> CreateWeather(CreateWeatherDTO createWeatherDto)
        {

            var location = await _context.Location.FindAsync(createWeatherDto.LocationId);
            if (location == null)
            {
                return null;
            }

            var weather = _mapper.Map<WeatherEntity>(createWeatherDto);
            weather.Location = location;


            _context.Weather.Add(weather);
            await _context.SaveChangesAsync();
            
            return _mapper.Map<WeatherDTO>(weather);;
        }


        public async Task<WeatherDTO?> UpdateWeather(int id, CreateWeatherDTO updateWeatherDto)
        {
            var weather = await _context.Weather
                .Include(w => w.Location)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (weather == null)
            {
                return null;
            }


            var location = await _context.Location.FindAsync(updateWeatherDto.LocationId);
            if (location == null)
            {
                return null;
            }


            _mapper.Map(updateWeatherDto, weather);
            weather.Location = location;

            await _context.SaveChangesAsync();

            return _mapper.Map<WeatherDTO>(weather);
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
            var weather = await _context.Weather.FindAsync(id);

            if (weather == null)
            {
                return false;
            }

            _context.Weather.Remove(weather);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}