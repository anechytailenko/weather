using System.Text.Json.Serialization;
using DNET.Backend.Api.Clients;
using DNET.Backend.Api.Filters;
using DNET.Backend.Api.Middleware;

using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.Api.Profiles;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ExceptionHandlerMiddleware>();
builder.Services.AddScoped<RateLimitMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductDb")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp:";
});

builder.Services.AddSingleton<IJwtValidator, JwtValidator>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { options.TokenValidationParameters = JwtValidator.CreateTokenValidationParameters(); });



builder.Services.AddHttpClient<IExternalWeatherApiClient, ExternalWeatherApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WeatherApi:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
});

builder.Services.AddScoped<IAlertService,AlertService>();
builder.Services.AddScoped<ILocationService,LocationService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<IWeatherService>(sp =>
{
    var weatherService = sp.GetRequiredService<WeatherService>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var logger = sp.GetRequiredService<ILogger<WeatherServiceWithCache>>();
    return new WeatherServiceWithCache(weatherService, cache, logger);
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ApiKeyFilter>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection("ApiKeys"));    
builder.Services.Configure<LocationServiceOptions>(builder.Configuration.GetSection("LocationService"));
builder.Services.Configure<AlertServiceOptions>(builder.Configuration.GetSection("AlertService"));
builder.Services.Configure<WeatherServiceOptions>(builder.Configuration.GetSection("WeatherService"));
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimitOptions"));
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(TimestampFilter));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddHostedService<MigrationService>();
builder.Services.AddHostedService<DeleteExpiredResetTokensService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();
app.Run();

public partial class Program
{
}

