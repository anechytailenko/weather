using System.Text.Json.Serialization;

namespace DNET.Backend.Api.DTOs
{
    public class CreateLocationDTO
    {
        public string City { get; set; }
        public string Country { get; set; }
        public List<int> AlertIds { get; set; } = new List<int>();
    }
}