using System.ComponentModel.DataAnnotations;

namespace DNET.Backend.Api.Requests;

public class LoginUserRequest
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}