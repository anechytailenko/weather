namespace DNET.Backend.Api.Models;

public class AuthResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public long Expiration { get; set; }

    public string? ErrorMessage { get; set; }

    
    public string Role { get; set; }

}