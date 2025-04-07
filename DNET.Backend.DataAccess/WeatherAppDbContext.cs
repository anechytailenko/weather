using System.Reflection;
using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;

namespace DNET.Backend.DataAccess;

public class WeatherAppDbContext : DbContext
{
    public DbSet<WeatherEntity> Weather { get; set; }
    public DbSet<AlertEntity> Alert { get; set; }
    public DbSet<LocationEntity> Location { get; set; }
    
    public DbSet<AlertLocationEntity> LocationAlerts { get; set; }
    public DbSet<UserEntity> User { get; set; }

    public WeatherAppDbContext(DbContextOptions<WeatherAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}