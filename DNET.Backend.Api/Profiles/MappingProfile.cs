using AutoMapper;
using DNET.Backend.DataAccess.Domain;
using DNET.Backend.Api.DTOs;

namespace DNET.Backend.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<AlertEntity, AlertDTO>()
                .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.AlertLocations.Select(al => new LocationDTO
                {
                    Id = al.Location.Id,
                    City = al.Location.City,
                    Country = al.Location.Country,
                    Weathers = null,
                    Alerts = null
                })));
            
            CreateMap<CreateAlertDTO, AlertEntity>()
                .ForMember(dest => dest.AlertLocations,
                    opt => opt.Ignore());
            
            
            
            CreateMap<LocationEntity, LocationDTO>()
                .ForMember(dest => dest.Weathers, opt => opt.MapFrom(src => src.Weathers.Select(w => new WeatherDTO
                {
                    Id = w.Id,
                    Temperature = w.Temperature,
                    Condition = w.Condition,
                    RecordedAt = w.RecordedAt,
                    Location = null
                })))
                .ForMember(dest => dest.Alerts, opt => opt.MapFrom(src => src.AlertLocations.Select(al => new AlertDTO
                {
                    Id = al.Alert.Id,
                    Message = al.Alert.Message,
                    IssuedAt = al.Alert.IssuedAt,
                    Locations = null
                })));
            
            CreateMap<CreateLocationDTO, LocationEntity>()
                .ForMember(dest => dest.Weathers, opt => opt.Ignore())
                .ForMember(dest => dest.AlertLocations,
                    opt => opt.Ignore());
            
            
            
            CreateMap<WeatherEntity, WeatherDTO>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location != null ? new LocationDTO
                {
                    Id = src.Location.Id,
                    City = src.Location.City,
                    Country = src.Location.Country,
                    Weathers = null,
                    Alerts = null
                } : null));
            
            
            CreateMap<CreateWeatherDTO, WeatherEntity>()
                .ForMember(dest => dest.Location, opt => opt.Ignore()); 
        }
    }
}



