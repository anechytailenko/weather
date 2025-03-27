using System;
using System.Text.Json.Serialization;

namespace DNET.Backend.Api.DTOs
{
    public class CreateAlertDTO
    {
        public string Message { get; set; }
        public DateTime IssuedAt { get; set; }
        public List<int> LocationIds { get; set; } = new List<int>();
    }
}