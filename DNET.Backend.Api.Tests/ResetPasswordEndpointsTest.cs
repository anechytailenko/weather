using DNET.Backend.Api.Controllers;
using DNET.Backend.Api.DTOs;
using DNET.Backend.Api.Requests;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DNET.Backend.Api.Tests;
public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _controller = new UserController(_mockUserService.Object);
    }
    
    
    [Fact]
    public async Task ForgotPassword_ValidEmail_ReturnsOk()
    {
       
        var request = new PasswordResetRequestDTO { Email = "user@example.com" }; 
        _mockUserService.Setup(x => x.GeneratePasswordResetToken(request.Email)).ReturnsAsync("generated_token");

        
        var result = await _controller.ForgotPassword(request);

        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("If an account exists, a reset link has been sent to email", ((dynamic)okResult.Value).Message); 
    }
    
    [Fact]
    public async Task ResetPassword_ValidRequest_ReturnsOk()
    {
       
        var request = new PasswordResetConfirmDTO 
        { 
            Token = "valid_token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };
    
        _mockUserService.Setup(x => x.ResetPassword(request.Token, request.NewPassword)).ReturnsAsync(true);

        
        var result = await _controller.ResetPassword(request);

       
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Password reset successful", 
            ((dynamic)okResult.Value).Message);
    }
    
    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
    {
        
        var request = new PasswordResetConfirmDTO 
        { 
            Token = "invalid_token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };
    
        _mockUserService.Setup(x => x.ResetPassword(request.Token, request.NewPassword)).ReturnsAsync(false);

       
        var result = await _controller.ResetPassword(request);

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid or expired token", 
            badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
    }
    
    
}