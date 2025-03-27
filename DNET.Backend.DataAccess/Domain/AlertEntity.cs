namespace DNET.Backend.DataAccess.Domain;

public class AlertEntity
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime IssuedAt { get; set; }

  
    public ICollection<AlertLocationEntity> AlertLocations { get; set; } = new List<AlertLocationEntity>();
}

