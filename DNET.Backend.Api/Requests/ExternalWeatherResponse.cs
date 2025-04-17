namespace DNET.Backend.Api.Requests;

public class ExternalWeatherResponse
{
    public double Temperature { get; set; }
    public string Condition { get; set; }
    public DateTime ObservationTime { get; set; }
    public string LocationName { get; set; }
}