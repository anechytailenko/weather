using System.ComponentModel.DataAnnotations;

namespace DNET.Backend.Api.DTOs;

public class PasswordResetConfirmDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;
        
    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
        
    [Required, Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}