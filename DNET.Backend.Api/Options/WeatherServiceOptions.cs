namespace DNET.Backend.Api.Options;

public class WeatherServiceOptions
{
    public int DefaultPaginationLimit { get; set; }
    public int DefaultPaginationOffset { get; set; }
    public string DefaultTemperatureUnit { get; set; }
}