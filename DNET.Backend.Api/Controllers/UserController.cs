using System.Security.Claims;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Requests;
using DNET.Backend.Api.Services;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNET.Backend.Api.Controllers;

[ApiController]
[Route("/auth/")]
public class UserController : ControllerBase
{
    
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] PasswordResetRequestDTO request)
    {
        await _userService.GeneratePasswordResetToken(request.Email);
        
        return Ok(new { Message = "If an account exists, a reset link has been sent to email" });
    }
    
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetConfirmDTO request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var success = await _userService.ResetPassword(request.Token, request.NewPassword);

        return success ? Ok(new { Message = "Password reset successful" }) : BadRequest(new { Message = "Invalid or expired token" });
    }
   

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var user = await _userService.RegisterUser(request);
        
        if (user == null) return NotFound(new { message = "User already exists" });

        return Ok(new { Message = "User registered successfully", UserId = user.Id });
    }

    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> GetUser()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
        if (userIdClaim == null)
            return Unauthorized(new { Message = "User not found" });
        
        var userId = Int32.Parse(userIdClaim.Value);
        
        var user = await _userService.GetUserById(userId);

        if (user == null)
            return NotFound(new { Message = "User not found" });

        return Ok(new UserDTO(user.Id, user.Email, user.FirstName, user.LastName, user.Role));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var authResult = await _userService.LoginUser(request);
        
        if (authResult == null) return Unauthorized(new { Message = "Invalid email or password" });


        return Ok(new { Message = "Login successful", AuthorizationToken = authResult.Token, Expiration = authResult.Expiration, RefreshToken = authResult.RefreshToken });
    }

    [HttpGet("login-with-google")]
    public IActionResult LoginWithGoogle()
    {
        string clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        
        return Redirect(
            $"https://accounts.google.com/o/oauth2/v2/auth?" +
            $"client_id={clientId}&" +
            "scope=openid%20profile%20email&" +
            "response_type=code&" +
            "redirect_uri=http%3A%2F%2Flocalhost%3A5000%2Fsignin-google");
    }
    
    [HttpGet("signin-google")]
    public async Task<IActionResult> LoginWithGoogleCallback([FromQuery] string? code)
    {
        var authResult = await _userService.LoginWithGoogle(code);
        if (authResult.ErrorMessage != null)
        {
            return BadRequest(new { Message = authResult.ErrorMessage });
        }

        return Ok(new { Message = "Login successful", AuthorizationToken = authResult.Token, Expiration = authResult.Expiration, RefreshToken = authResult.RefreshToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        await _userService.InvalidateToken(request.RefreshToken);
        return NoContent();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var authResult = await _userService.GenerateNewJwtToken(request.RefreshToken);
        if (authResult == null) return Unauthorized(new { Message = "Invalid refresh token" });
        return Ok(authResult);

    }
}