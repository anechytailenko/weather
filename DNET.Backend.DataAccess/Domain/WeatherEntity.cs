namespace DNET.Backend.DataAccess.Domain;

public class WeatherEntity
{
    public int Id { get; set; }
    public double Temperature { get; set; }
    public string Condition { get; set; }
    public DateTime RecordedAt { get; set; }


    public int LocationId { get; set; }
    public LocationEntity Location { get; set; }
}
