namespace DNET.Backend.DataAccess.Domain;

public class PasswordResetTokenEntity
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; }
}