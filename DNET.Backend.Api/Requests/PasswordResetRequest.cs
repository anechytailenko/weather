using System.ComponentModel.DataAnnotations;

namespace DNET.Backend.Api.Requests;

public class PasswordResetRequestDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}
