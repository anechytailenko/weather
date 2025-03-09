using System.Net;
using DNET.Backend.Api.DB;
using System.Text;
using System.Text.Json;
using Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DNET.Backend.Api.Tests;

public sealed class LocationApiTests : BaseApiTests
{
    public LocationApiTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GetLocation_ShouldReturnAllLocationData()
    {
        var response = await Client.GetAsync("/location");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var records = await response.Content.ReadFromJsonAsync<List<Location>>();
        Assert.NotNull(records);
    }
    
    [Fact]
    public async Task GetLocationById_ShouldReturnLocationData_WhenExists()
    {
        var response = await Client.GetAsync("/location/2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var location = await response.Content.ReadFromJsonAsync<Location>();
        Assert.NotNull(location);
        Assert.Equal("Los Angeles", location.City);
    }
    
    [Fact]
    public async Task GetLocationById_ShouldReturnNotFound_WhenLocationDoesNotExist()
    {
        var response = await Client.GetAsync("/location/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateLocation_ShouldReturnCreated()
    {
        var newLocation = new Location ("Kiev","Ukraine" );

        var response = await Client.PostAsJsonAsync("/location", newLocation);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = await response.Content.ReadFromJsonAsync<Location>();
        Assert.NotNull(location);
        Assert.Equal("Kiev", location.City);
    }
    
    [Fact]
    public async Task DeleteLocation_ShouldReturnNoContent_WhenLocationExists()
    {
        var response = await Client.DeleteAsync("/location/10");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteLocation_ShouldReturnNoFound_WhenLocationNotExist()
    {
        var response = await Client.DeleteAsync("/location/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocation_ShouldReturnUpdatedData_WhenLocationExist()
    {
        var updatedLocation = new Location("Lviv", "Ukraine");
        
        var content = new StringContent(
            JsonSerializer.Serialize(updatedLocation),
            Encoding.UTF8,
            "application/json"
        );
        
        
        var response1 = await Client.PutAsync("/location/1",content);
        var response2 = await Client.GetAsync("/location/1");
        var location = await response2.Content.ReadFromJsonAsync<Location>();
        Assert.Equal("Lviv", location.City);
       
    }
    
    
}