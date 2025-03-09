using Microsoft.AspNetCore.Mvc;
using Models;
using DNET.Backend.Api.DB;
using Services;

namespace Controllers
{
    public static class LocationController
    {
        public static void AddApiRoute(WebApplication app)
        {
            var group = app.MapGroup("/location");
            
            group.MapGet("/", ( LocationService locationService) => Results.Ok(locationService.GetAllLocations()));
            
            group.MapGet("/{id:int}", (int id, LocationService locationService) =>
            {
                var location = locationService.GetLocationById(id);
                return location != null ? Results.Ok(location) : Results.NotFound();
            });
            
            group.MapPost("/", (Location location, LocationService locationService) =>
            {
                var newLocation = locationService.Create(location);
                return Results.Created($"/location/{newLocation.Id}", newLocation);
            });
            
            group.MapDelete("/{id:int}", (int id, LocationService locationService) =>
            {
                return locationService.DeleteLLocationById(id) ? Results.NoContent() : Results.NotFound();
            });
            
            group.MapPut("/{id:int}", (int id, Location location, LocationService locationService) =>{
                return locationService.UpdateEntirelyLocation(id,location) != null ? Results.Ok(location) : Results.NotFound();
            });
        }
    }
    
}