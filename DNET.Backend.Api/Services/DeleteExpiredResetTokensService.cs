using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Services;

public class DeleteExpiredResetTokensService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeleteExpiredResetTokensService> _logger;

    public DeleteExpiredResetTokensService(IServiceScopeFactory scopeFactory,ILogger<DeleteExpiredResetTokensService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started background service for deleting expired reset tokens.");
        while (true)
        {
            try
            {
                if (stoppingToken.IsCancellationRequested)
                    break;
                
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<WeatherAppDbContext>();
            
                var expiredPasswordResetTokens = dbContext.PasswordResetToken.Where(e => e.Expires < DateTime.UtcNow);
                
                dbContext.PasswordResetToken.RemoveRange(expiredPasswordResetTokens);
                await dbContext.SaveChangesAsync(stoppingToken);
        
                _logger.LogInformation("Successfully removed expired tokens."); 
                await Task.Delay(60000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting expired reset tokens.");
            }
        }
        
    }
}