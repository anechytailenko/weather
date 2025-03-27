using AutoMapper;
using DNET.Backend.Api.Profiles;
using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Tests;

public static class Utils
{
    private static readonly Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
    {
        var config = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile<MappingProfile>();
        });
        return config.CreateMapper();
    });
    
    
    
    public static WeatherAppDbContext CreateInMemoryDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<WeatherAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableDetailedErrors()
            .Options;

        var databaseContext = new WeatherAppDbContext(options); 

        databaseContext.Database.EnsureCreated();

        return databaseContext;
    }
    

    public static IMapper Get() => _mapper.Value;
}






