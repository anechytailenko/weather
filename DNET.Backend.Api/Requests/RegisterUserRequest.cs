using System.ComponentModel.DataAnnotations;

namespace DNET.Backend.Api.Requests;

public class RegisterUserRequest
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    
    public string Role { get; set; } = "User";
}