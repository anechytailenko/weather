using Microsoft.AspNetCore.Mvc;
using Models;
using DNET.Backend.Api.DB;
using Services;

namespace Controllers
{
    public static class AlertController
    {
        public static void AddApiRoute(WebApplication app)
        {
            var group = app.MapGroup("/alert");
            
            group.MapGet("/", ( AlertService alertService) => Results.Ok(alertService.GetAllAlert()));
            
            group.MapGet("/{id:int}", (int id, AlertService alertService) =>
            {
                var alert = alertService.GetAlertById(id);
                return alert != null ? Results.Ok(alert) : Results.NotFound();
            });
            
            group.MapPost("/", (Alert alert, AlertService alertService) =>
            {
                var newAlert = alertService.Create(alert);
                return Results.Created($"/alert/{newAlert.Id}", newAlert);
            });
            
            group.MapDelete("/{id:int}", (int id,  AlertService alertService) =>
            {
                return alertService.DeleteLAlertById(id) ? Results.NoContent() : Results.NotFound();
            });
            
            group.MapPut("/{id:int}", (int id, Alert alert, AlertService alertService) =>{
                return alertService.UpdateEntirelyAlert(id,alert) != null ? Results.Ok(alert) : Results.NotFound();
            });
        }
    }
    
}