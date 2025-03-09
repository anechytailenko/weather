using System.Net;
using DNET.Backend.Api.DB;
using Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

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
        var newAlert = new Alert(1, "High temperature",DateTime.Now);

        var response = await Client.PostAsJsonAsync("/alert", newAlert);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var alert = await response.Content.ReadFromJsonAsync<Alert>();
        Assert.NotNull(alert);
        Assert.Equal("High temperature", alert.Message);
    }
    
    [Fact]
    public async Task DeleteAlert_ShouldReturnNoContent_WhenLocationExists()
    {
        var response = await Client.DeleteAsync("/alert/9");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteAlert_ShouldReturnNoFound_WhenLocationNotExist()
    {
        var response = await Client.DeleteAsync("/alert/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

  
  
    
    
}