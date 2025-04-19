using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Services;

public class MigrationService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MigrationService> _logger;
    
    public MigrationService(IServiceScopeFactory scopeFactory, ILogger<MigrationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migration service started.");
        
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WeatherAppDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migration completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred during migrations.");
            
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}