using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNET.Backend.DataAccess.Configurations;

public class WeatherEntityConfiguration : IEntityTypeConfiguration<WeatherEntity>
{
    public void Configure(EntityTypeBuilder<WeatherEntity> builder)
    {
        builder.ToTable("weather");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.LocationId)
            .HasColumnName("location_id")
            .IsRequired();
        
        
        builder.Property(e => e.Condition)
            .HasColumnName("condition")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(e => e.Temperature)
            .HasColumnName("temperature")
            .IsRequired();

        builder.Property(e => e.RecordedAt)
            .HasColumnName("recorded_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        
        builder.HasOne(w => w.Location)
            .WithMany(l => l.Weathers)
            .HasForeignKey(w => w.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}