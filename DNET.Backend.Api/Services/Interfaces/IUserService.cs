using System.Text.Json;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Requests;

namespace DNET.Backend.Api.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO?> GetUserById(int id);
    Task<UserDTO?> GetUserByEmail(string email);
    Task<UserDTO?> RegisterUser(RegisterUserRequest request);
    Task<UserDTO?> RegisterUser(RegisterUserRequest request, string provider);
    Task<AuthResult?> LoginUser(LoginUserRequest request);

    Task<AuthResult> LoginWithGoogle(string? code);
    Task<AuthResult?> GenerateNewJwtToken(string refreshToken);
    Task InvalidateToken(string refreshToken);

    
    Task<string?> GeneratePasswordResetToken(string email);
    Task<bool> ResetPassword(string resetCode, string newPassword);

}