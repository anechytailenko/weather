namespace DNET.Backend.DataAccess.Domain;

public class AlertLocationEntity
{
    public int AlertId { get; set; }
    public AlertEntity Alert { get; set; }

    public int LocationId { get; set; }
    public LocationEntity Location { get; set; }

   
}