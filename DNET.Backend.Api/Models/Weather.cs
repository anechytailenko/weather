namespace Models;

public class Weather
{ 
    public int Id { get; set; }
    public int LocationId { get; set; }
    public double Temperature { get; set; }
    public string Condition { get; set; }  
    public DateTime RecordedAt { get; set; }
}