using DNET.Backend.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.Api.Services;

public class DeleteExpiredResetTokensService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DeleteExpiredResetTokensService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
        
                await Task.Delay(60000, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
    }
}