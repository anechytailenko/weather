using System.Text.Json.Serialization;
namespace Models
{
    public class Alert
    {   
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string Message { get; set; }
        public DateTime IssuedAt { get; set; }

        public Alert(int id,int locationId, string message, DateTime issuedAt)
        {
            Id = id;
            LocationId = locationId;
            Message = message;
            IssuedAt = issuedAt;
        }
        
        [JsonConstructor]
        public Alert(int locationId,string message, DateTime issuedAt)
        {
            LocationId = locationId;
            Message = message;
            IssuedAt = issuedAt;
        }
    }
}