using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AlertLocationEntityConfiguration : IEntityTypeConfiguration<AlertLocationEntity>
{
    public void Configure(EntityTypeBuilder<AlertLocationEntity> builder)
    {
        builder.ToTable("location_alert");
        
        builder.Property(la => la.AlertId)
            .HasColumnName("alert_id");

        builder.Property(la => la.LocationId)
            .HasColumnName("location_id");
        
        builder.HasKey(la => new { la.LocationId, la.AlertId });
        
        builder.HasOne(al => al.Alert)
            .WithMany(a => a.AlertLocations)
            .HasForeignKey(al => al.AlertId);

        builder.HasOne(al => al.Location)
            .WithMany(l => l.AlertLocations)
            .HasForeignKey(al => al.LocationId);
    }
}