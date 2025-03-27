using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNET.Backend.DataAccess.Configurations;

public class LocationEntityConfiguration : IEntityTypeConfiguration<LocationEntity>
{
    public void Configure(EntityTypeBuilder<LocationEntity> builder)
    {
        builder.ToTable("location");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.City)
            .HasColumnName("city")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Country)
            .HasColumnName("country")
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasMany(l => l.Weathers)
            .WithOne(w => w.Location)
            .HasForeignKey(w => w.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.HasMany(l => l.AlertLocations)
            .WithOne(al => al.Location)
            .HasForeignKey(al => al.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}