using System.Text.Json.Serialization;
using DNET.Backend.DataAccess.Domain;

namespace DNET.Backend.Api.DTOs
{ 
    public class LocationDTO
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
        public List<WeatherDTO> Weathers { get; set; } = new List<WeatherDTO>();
        public List<AlertDTO> Alerts { get; set; } = new List<AlertDTO>();
    }
}

