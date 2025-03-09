using Controllers;
using Services;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<LocationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


AlertController.AddApiRoute(app);
LocationController.AddApiRoute(app);

int lastWeatherId = 0;
var weatherData = new List<WeatherData>();

#region API

app.MapGet("/", () => "Hello World!");

// GET /weather
app.MapGet("/weather", (HttpContext context) =>
{
    var query = context.Request.Query;
    
    int.TryParse(query["limit"], out var limit);
    int.TryParse(query["offset"], out var offset);

    if (limit == 0)
    {
        limit = weatherData.Count;
    }
    
    return Results.Ok(weatherData.Skip(offset).Take(limit).ToArray());
});

// GET /weather/1
app.MapGet("/weather/{id:int}", (int id) =>
{
    var record = weatherData.Find(x => x.Id == id);
    if (record == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(record);
});

// POST /weather
app.MapPost("/weather", (WeatherData data) =>
{
    lastWeatherId++;
    var newId = lastWeatherId;
    data.Id = newId;
    weatherData.Add(data);
    return Results.Created($"/weather/{newId}", data);
});

// PUT /weather/1
app.MapPut("/weather/{id:int}", (int id, WeatherData data) =>
{
    var record = weatherData.Find(x => x.Id == id);
    if (record == null)
    {
        return Results.NotFound();
    }
    
    record.LocationId = data.LocationId;
    record.Temperature = data.Temperature;
    record.Condition = data.Condition;
    record.RecordedAt = data.RecordedAt;
    
    return Results.Ok(data);
});

// PATCH /weather/1
app.MapPatch("/weather/{id:int}", (int id, JsonElement patch) =>
{
    var record = weatherData.Find(x => x.Id == id);
    if (record == null)
    {
        return Results.NotFound();
    }
    
    foreach (var field in patch.EnumerateObject())
    {
        switch (field.Name)
        {
            case "locationId":
                record.LocationId = field.Value.GetInt32();
                break;
            case "temperature":
                record.Temperature = field.Value.GetDouble();
                break;
            case "condition":
                record.Condition = field.Value.GetString();
                break;
            case "recordedAt":
                record.RecordedAt = field.Value.GetDateTime();
                break;
        }
    }
    
    return Results.Ok(record);
});

// DELETE /products/1
app.MapDelete("/weather/{id:int}", (int id) =>
{
    var record = weatherData.Find(x => x.Id == id);
    if (record == null)
    {
        return Results.NotFound();
    }

    weatherData.Remove(record);
    return Results.NoContent();
});

#endregion

app.Run();

public class WeatherData
{ 
    public int Id { get; set; }
    public int LocationId { get; set; }
    public double Temperature { get; set; }
    public string Condition { get; set; }  
    public DateTime RecordedAt { get; set; }
}

public partial class Program
{
}

