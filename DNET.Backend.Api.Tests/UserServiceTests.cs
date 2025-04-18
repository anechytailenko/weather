using System.Net;
using System.Security.Claims;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Requests;
using DNET.Backend.Api.Services;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using DNET.Backend.DataAccess.Domain;
using Microsoft.AspNetCore.Http;
using Moq;

namespace DNET.Backend.Api.Tests;

[Collection("Sequential")]
public class UserServiceTests : IAsyncLifetime
{
    private UserService _userService;
    private WeatherAppDbContext _contextMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<IJwtValidator> _jwtValidatorMock;
    private Mock<IEmailService> _emailServiceMock;
    
    public async Task InitializeAsync()
    {
        _contextMock = Utils.CreateInMemoryDatabaseContext();
        _contextMock.User.Add(new UserEntity { Email = "test1@gmail.com", FirstName = "Test1First", LastName = "Test1Last", PasswordHash = UserService.Hash("1234", "dfkld"), PasswordSalt = "dfkld", Role = "User", RefreshTokenHash = UserService.Hash("refreshToken1"), RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(5), IpAddress = "127.0.0.1", UserAgent = "testUserAgent"});
        _contextMock.User.Add(new UserEntity { Email = "test2@gmail.com", FirstName = "Test2First", LastName = "Test2Last", PasswordHash = UserService.Hash("hello", "23dfs"), PasswordSalt = "23dfs", Role = "Admin", RefreshTokenHash = UserService.Hash("refreshToken2"), RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(-1), IpAddress = "127.0.0.1", UserAgent = "testUserAgent"});
        await _contextMock.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenExists()
    {
        BasicMock();
        InitService();
        var user = await _userService.GetUserById(1); 
        Assert.NotNull(user);
        Assert.Equal("test1@gmail.com", user.Email);
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnNull_WhenNotExists()
    {
        BasicMock();
        InitService();
        var user = await _userService.GetUserById(9999);
        Assert.Null(user);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnUser_WhenUserNotExists()
    {
        BasicMock();
        _jwtValidatorMock.Setup(x => x.CreateRefreshToken()).Returns("newRefreshToken");
        InitService();
        var newUser = new RegisterUserRequest { Email = "test3@gmail.com", FirstName = "test3first", LastName = "test3last", Password = "randomPassword", Role = "User"};
        var user = await _userService.RegisterUser(newUser);
        Assert.NotNull(user);
        Assert.Equal("test3@gmail.com", user.Email);
        Assert.Equal("test3first", user.FirstName);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnNull_WhenUserExists()
    {
        BasicMock();
        InitService();
        var existingUser = await _userService.GetUserById(1);
        var fakeNewUser = new RegisterUserRequest { Email = existingUser.Email, FirstName = existingUser.FirstName, LastName = existingUser.LastName, Password = "sldjkf" };
        var user = await _userService.RegisterUser(fakeNewUser);
        Assert.Null(user);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnAccessTokens_WhenUserExistsAndPasswordMatches()
    {
        BasicMock();
        _jwtValidatorMock.Setup(x => x.CreateJwtToken(It.IsAny<List<Claim>>())).Returns(new AuthResult
        {
            Token = "expectedToken",
            Expiration = 15 * 60,
            RefreshToken = "expectedRefreshToken"
        });
        InitService();

        var user = new LoginUserRequest { Email = "test1@gmail.com", Password = "1234" };
        var authResult = await _userService.LoginUser(user);
        Assert.NotNull(authResult);
        Assert.Equal("expectedToken", authResult.Token);
        Assert.Equal("expectedRefreshToken", authResult.RefreshToken);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnNull_WhenUserDoesNotExist()
    {
        BasicMock();
        InitService();   
        var user = new LoginUserRequest { Email = "notExistingEmail@gmail.com", Password = "34873" };
        var authResult = await _userService.LoginUser(user);
        Assert.Null(authResult);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnNull_WhenWrongPassword()
    {
        BasicMock();
        InitService();
        var user = new LoginUserRequest { Email = "test1@gmail.com", Password = "wrongPassword" };
        var authResult = await _userService.LoginUser(user);
        Assert.Null(authResult);
    }

    [Fact]
    public async Task GenerateNewJwtToken_ShouldReturnAccessToken_WhenRefreshTokenIsValid()
    {
        BasicMock();
        _jwtValidatorMock.Setup(x => x.CreateJwtToken(It.IsAny<List<Claim>>())).Returns(new AuthResult
        {
            Token = "newAccessToken",
            Expiration = 15 * 60
        });
        InitService();

        var authResult = await _userService.GenerateNewJwtToken("refreshToken1");
        Assert.NotNull(authResult);
        Assert.NotNull(authResult.Token);
    }

    [Fact]
    public async Task GenerateNewJwtToken_ShouldReturnNull_WhenRefreshTokenIsIncorrect()
    {
        BasicMock();
        InitService();
        
        var authResult = await _userService.GenerateNewJwtToken("incorrectRefreshToken");
        Assert.Null(authResult);
    }

    [Fact]
    public async Task GenerateNewJwtToken_ShouldReturnNull_WhenRefreshTokenIsExpired()
    {
        BasicMock();
        InitService();
        
        var authResult = await _userService.GenerateNewJwtToken("refreshToken2");
        Assert.Null(authResult);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_ShouldReturnResetToken_WhenEmailExists()
    {
        BasicMock();
        InitService();

        var token = await _userService.GeneratePasswordResetToken("test1@gmail.com");
        Assert.NotNull(token);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        BasicMock();
        InitService();

        var token = await _userService.GeneratePasswordResetToken("notExistentEmail@gmail.com");
        Assert.Null(token);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnTrue_WhenResetTokenIsValid()
    {
        BasicMock();
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _contextMock.PasswordResetToken.Add(new PasswordResetTokenEntity { Expires = DateTime.Now.AddMinutes(5), UserId = 1, Token = "correctResetToken"});
        await _contextMock.SaveChangesAsync();
        InitService();
        
        var result = await _userService.ResetPassword("correctResetToken", "newPassword");
        Assert.True(result);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnFalse_WhenResetTokenIsIncorrect()
    {
        BasicMock();
        InitService();
        var result = await _userService.ResetPassword("incorrectResetToken", "newPassword");
        Assert.False(result);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnFalse_WhenResetTokenIsExpired()
    {
        BasicMock();
        InitService();
        _contextMock.PasswordResetToken.Add(new PasswordResetTokenEntity { Expires = DateTime.Now.AddMinutes(-1), UserId = 2, Token = "correctResetToken2"});
        await _contextMock.SaveChangesAsync();
        var result = await _userService.ResetPassword("correctResetToken2", "newPassword");
        Assert.False(result);
    }
    
    private void BasicMock()
    {
        _jwtValidatorMock = new Mock<IJwtValidator>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _emailServiceMock = new Mock<IEmailService>();
        
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        context.Request.Headers.UserAgent = "testUserAgent";
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
    }

    private void InitService()
    {
        _userService = new UserService(_contextMock, _jwtValidatorMock.Object, _httpContextAccessorMock.Object, _emailServiceMock.Object);
    }
}