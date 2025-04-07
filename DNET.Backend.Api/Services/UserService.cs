using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Requests;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Services;

public class UserService : IUserService
{
    private readonly WeatherAppDbContext _context;
    private readonly IJwtValidator _jwtValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserService(WeatherAppDbContext context, IJwtValidator jwtValidator, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _jwtValidator = jwtValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserDTO?> GetUserById(int id)
    {
        var user = await _context.User.FirstOrDefaultAsync(user => user.Id == id);
        return user == null ? null : new UserDTO(user);
    }
    public async Task<UserDTO?> GetUserByEmail(string email)
    {
        var user = await _context.User.FirstOrDefaultAsync(user => user.Email == email);
        return user == null ? null : new UserDTO(user);
    }
    public async Task<UserDTO?> RegisterUser(RegisterUserRequest request)
    {
        var trimmedEmail = request.Email.Trim().ToLower();

        var existingUser = _context.User
            .FirstOrDefault(u => u.Email == trimmedEmail);

        if (existingUser != null) return null;

        var salt = Guid.NewGuid().ToString();
        
        var refreshToken = _jwtValidator.CreateRefreshToken();
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        var userAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();
        
        var user = new UserEntity
        {
            Email = trimmedEmail,
            PasswordHash = Hash(request.Password, salt),
            PasswordSalt = salt,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RefreshTokenHash = Hash(refreshToken),
            RefreshTokenExpiration = DateTime.UtcNow.AddHours(2),
            Role = request.Role,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO(user);
    }
    public async Task<UserDTO?> RegisterUser(RegisterUserRequest request, string provider)
    {
        var existingUser = _context.User.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null) return null;
        
        var refreshToken = _jwtValidator.CreateRefreshToken();
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        var userAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();
        
        var user = new UserEntity()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            LoginProvider = provider,
            RefreshTokenHash = Hash(refreshToken),
            RefreshTokenExpiration = DateTime.UtcNow.AddHours(2),
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
        
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return new UserDTO(user);
    }

    public async Task<AuthResult?> LoginUser(LoginUserRequest request)
    {
        var trimmedEmail = request.Email.Trim().ToLower();

        var user = await _context.User
            .FirstOrDefaultAsync(u => u.Email == trimmedEmail);

        if (user == null) return null;

        var hashedPassword = Hash(request.Password, user.PasswordSalt);
        if (user.PasswordHash != hashedPassword) return null;

        var claims = CreateClaims(user);

        var authResult = _jwtValidator.CreateJwtToken(claims);
        
        user.RefreshTokenHash = Hash(authResult.RefreshToken);
        user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(2);
        user.IpAddress =  _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        user.UserAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();
        
        await _context.SaveChangesAsync();
        
        return authResult;
    }

    public async Task<AuthResult> LoginWithGoogle(string? code)
    {
        if (string.IsNullOrEmpty(code))
            return new AuthResult { ErrorMessage = "Invalid code" };

        var httpClient = new HttpClient();

        var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")!;
        var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")!;
        
        var tokenResponse = httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", "http://localhost:5000/signin-google"),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        })).Result;

        if (!tokenResponse.IsSuccessStatusCode)
            return new AuthResult{ ErrorMessage = "Invalid token response" };

        var tokenResponseContent = tokenResponse.Content.ReadAsStringAsync().Result;
        var tokenResponseJson = System.Text.Json.JsonDocument.Parse(tokenResponseContent);

        var accessToken = tokenResponseJson.RootElement.GetProperty("access_token").GetString();

        var userInfoResponse = new HttpClient()
            .GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}").Result;

        if (!userInfoResponse.IsSuccessStatusCode)
            return new AuthResult{ ErrorMessage = "Invalid user info response" };

        var userInfoContent = userInfoResponse.Content.ReadAsStringAsync().Result;
        var userInfoJson = System.Text.Json.JsonDocument.Parse(userInfoContent);

        var email = userInfoJson.RootElement.GetProperty("email").GetString();
        var firstName = userInfoJson.RootElement.GetProperty("given_name").GetString();
        var lastName = userInfoJson.RootElement.GetProperty("family_name").GetString();

        var trimmedEmail = email.ToLower();

        var userDto = await GetUserByEmail(email) ?? 
                   await RegisterUser(new RegisterUserRequest() { Email = trimmedEmail, FirstName = firstName, LastName = lastName}, "Google");
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userDto.Email);

        var claims = CreateClaims(user);

        var authResult = _jwtValidator.CreateJwtToken(claims);
        
        user.RefreshTokenHash = Hash(authResult.RefreshToken);
        user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(2);
        user.IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        user.UserAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();
        
        await _context.SaveChangesAsync();
        
        return authResult;
    }

    public async Task<AuthResult?> GenerateNewJwtToken(string refreshToken)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.RefreshTokenHash == Hash(refreshToken, ""));
        if (user == null) return null;
        if (user.RefreshTokenExpiration < DateTime.UtcNow) return null;
        
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        var userAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();

        if (user.IpAddress != ipAddress || user.UserAgent != userAgent)
        {
            await InvalidateToken(refreshToken);
            return null;
        }

        var claims = CreateClaims(user);
        
        var authResult = _jwtValidator.CreateJwtToken(claims);
        return new AuthResult { Token = authResult.Token, Expiration = authResult.Expiration };
    }

    public async Task InvalidateToken(string refreshToken)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.RefreshTokenHash == Hash(refreshToken, ""));
        if (user == null) return;
        user.RefreshTokenExpiration = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return;
    }
    
    public async Task<string?> GeneratePasswordResetToken(string email)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
            
        if (user == null) return null;

        
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddMinutes(15);
            
        await _context.SaveChangesAsync();
            
       
            
        return token;
    }

    public async Task<bool> ResetPassword(string resetCode, string newPassword)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => 
            u.PasswordResetToken == resetCode && u.PasswordResetTokenExpires > DateTime.UtcNow);

        if (user == null) return false;

        
        var newSalt = Guid.NewGuid().ToString();
        user.PasswordSalt = newSalt;
        user.PasswordHash = Hash(newPassword, newSalt);
            
        
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;
            
        await _context.SaveChangesAsync();
        return true;
    }
    
    private static string Hash(string password, string salt="")
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
        return Convert.ToBase64String(hash);
    }
    
    private static List<Claim> CreateClaims(UserEntity user)
    {
        return new List<Claim>
        {
            new("id", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role)
        };
    }
}
    
    
    
    
    
    
