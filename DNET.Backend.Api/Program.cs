using DNET.Backend.Api.Models;
using DNET.Backend.Api.Options;
using DNET.Backend.Api.Services;
using DNET.Backend.Api.Services.Interfaces;
using Services;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAlertService,AlertService>();
builder.Services.AddScoped<ILocationService,LocationService>();
builder.Services.AddControllers();
builder.Services.Configure<LocationServiceSettings>(builder.Configuration.GetSection("LocationServiceSettings"));
builder.Services.Configure<AlertServiceSettings>(builder.Configuration.GetSection("AlertServiceSettings"));
builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.Configure<WeatherServiceOptions>(builder.Configuration.GetSection("WeatherService"));
builder.Services.AddControllers();

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

