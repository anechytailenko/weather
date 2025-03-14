using System.Net;
using System.Net.Http.Json;
using DNET.Backend.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using System.Net.Http.Json;

using System.Text;



namespace DNET.Backend.Api.Tests;

public sealed class AlertApiTests : BaseApiTests
{
    public AlertApiTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GetAlert_ShouldReturnAllLocationData()
    {
        var response = await Client.GetAsync("/alert");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var records = await response.Content.ReadFromJsonAsync<List<Alert>>();
        Assert.NotNull(records);
    }
    
    [Fact]
    public async Task GetAlertById_ShouldReturnLocationData_WhenExists()
    {
        var response = await Client.GetAsync("/alert/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var alert = await response.Content.ReadFromJsonAsync<Alert>();
        Assert.NotNull(alert);
        Assert.Equal("Fire hazard detected", alert.Message);
    }
    
    [Fact]
    public async Task GeAlertById_ShouldReturnNotFound_WhenLocationDoesNotExist()
    {
        var response = await Client.GetAsync("/alert/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAlert_ShouldReturnCreated()
    {
        var newAlert = new Alert(1,"High temperature",DateTime.Now);

        var response = await Client.PostAsJsonAsync("/alert", newAlert);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var alert = await response.Content.ReadFromJsonAsync<Alert>();
        Assert.NotNull(alert);
        Assert.Equal("High temperature", alert.Message);
    }
  
    
    
    // uncomment if the "EnableDelete": true
    //[Fact]
    //public async Task DeleteAlert_ShouldReturnNoContent_WhenLocationExists()
    //{
        //var response = await Client.DeleteAsync("/alert/9");

        //Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    //}
    
    
    // uncomment if the "EnableDelete": true
    //[Fact]
    //public async Task DeleteAlert_ShouldReturnNoFound_WhenLocationNotExist()
    //{
        //var response = await Client.DeleteAsync("/alert/99");

        //Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    //}

    
    // comment if the "EnableDelete": false
    [Fact]
    public async Task DeleteAlert_ShouldReturnConflict_When_EnableDelete_SetFalse()
    {
         var response = await Client.DeleteAsync("/alert/10");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
     }
    
    
    [Fact]
    public async Task UpdateAlert_ShouldReturnUpdatedData_WhenLocationExist()
    {
        var updatedAlert = new Alert( 4, "Severe weather warning", DateTime.Now);
        
        var content = new StringContent(
            JsonSerializer.Serialize(updatedAlert),
            Encoding.UTF8,
            "application/json"
        );
        
        
        var response1 = await Client.PutAsync("/alert/3",content);
        var response2 = await Client.GetAsync("/alert/3");
        var alert = await response2.Content.ReadFromJsonAsync<Alert>();
        Assert.Equal(4, alert?.LocationId);
       
    }
  
  
    
    
}