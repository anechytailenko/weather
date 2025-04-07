namespace DNET.Backend.DataAccess.Domain;

public class UserEntity
{
    public int Id { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string? LoginProvider { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; } 

    public string? RefreshTokenHash { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiration { get; set; }

    
    public string Role { get; set; } = "User";
    
    public string? PasswordResetToken { get; set; }
    
    public DateTime? PasswordResetTokenExpires { get; set; }
    
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}