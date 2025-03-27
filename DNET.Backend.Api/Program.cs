using System.Text.Json.Serialization;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using DNET.Backend.Api.Services.Interfaces;
using DNET.Backend.Api.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WeatherAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WeatherAppDb"))
);
builder.Services.AddScoped<IAlertService,AlertService>();
builder.Services.AddScoped<ILocationService,LocationService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
builder.Services.Configure<LocationServiceOptions>(builder.Configuration.GetSection("LocationService"));
builder.Services.Configure<AlertServiceOptions>(builder.Configuration.GetSection("AlertService"));
builder.Services.Configure<WeatherServiceOptions>(builder.Configuration.GetSection("WeatherService"));
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddControllers();
// run migration by start
// using var scope = app.Services.CreateScope();
// var dbContext = scope.ServiceProvider.GetRequiredService<WeatherAppDbContext>();
// dbContext.Database.Migrate();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program
{
}

