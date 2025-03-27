using System;
using System.Text.Json.Serialization;

namespace DNET.Backend.Api.DTOs
{
    public class CreateWeatherDTO
    {
        public int LocationId { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; }
        public DateTime RecordedAt { get; set; }
        
    }
}