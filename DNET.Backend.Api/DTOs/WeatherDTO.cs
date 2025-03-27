using DNET.Backend.DataAccess.Domain;

namespace DNET.Backend.Api.DTOs
{
    public class WeatherDTO
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; }
        public DateTime RecordedAt { get; set; }
        public LocationDTO Location { get; set; }
    }
}