using System.Text.Json.Serialization;
using DNET.Backend.DataAccess.Domain;

namespace DNET.Backend.Api.DTOs
{
    public class AlertDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime IssuedAt { get; set; }
        
        public List<LocationDTO> Locations { get; set; } = new List<LocationDTO>();
    }
}