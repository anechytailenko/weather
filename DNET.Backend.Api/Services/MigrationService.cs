using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Services;

public class MigrationService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public MigrationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Migration service started");
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WeatherAppDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Migration service completed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}