using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DNET.Backend.Api.Services;

public class JwtValidator : IJwtValidator
{
    private const string Token = "your_secret_key_should_be_long_enough_at_least_512_bits_long_to_secure_the_token";
    private readonly ILogger<JwtValidator> _logger;

    
    public JwtValidator(ILogger<JwtValidator> logger)
    {
        _logger = logger;
    }
    
    public AuthResult CreateJwtToken(List<Claim> claims)
    {
        var expiration = 15 * 60; // 15 minutes
        _logger.LogInformation("Creating JWT token with expiration {Expiration} seconds", expiration);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(expiration),
            SigningCredentials = new SigningCredentials(CreateSecurityKey(), SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = CreateRefreshToken();
        
        _logger.LogInformation("JWT token created successfully with refresh token");
        return new AuthResult
        {
            Token = tokenHandler.WriteToken(token),
            Expiration = expiration,
            RefreshToken = refreshToken
        };
    }

    public string CreateRefreshToken()
    {
        var refreshToken = Guid.NewGuid().ToString();
        _logger.LogInformation("Generated new refresh token");
        return refreshToken;
    }

    public static TokenValidationParameters CreateTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = CreateSecurityKey(),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role, 
            ValidateLifetime = true,
            LifetimeValidator = (notBefore, expires, token, parameters) =>
            {
                if (expires == null)
                    return false;

                return expires > DateTime.UtcNow;
            }
        };
    }

    private static SymmetricSecurityKey CreateSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Token));
    }
}
