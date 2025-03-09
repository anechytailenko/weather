
using System.Text.Json.Serialization;
namespace Models

{
    public class Location
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

       
        public Location(int id, string city, string country)
        {
            Id = id;
            City = city;
            Country = country;
        }
        [JsonConstructor]
        public Location(string city, string country)
        {
            City = city;
            Country = country;
        }
    }
}