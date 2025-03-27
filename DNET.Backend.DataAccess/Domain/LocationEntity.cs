namespace DNET.Backend.DataAccess.Domain;

public class LocationEntity
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }


    public ICollection<WeatherEntity> Weathers { get; set; } = new List<WeatherEntity>();
    
    public ICollection<AlertLocationEntity> AlertLocations { get; set; } = new List<AlertLocationEntity>();
}

