using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNET.Backend.DataAccess.Configurations;

public class AlertEntityConfiguration : IEntityTypeConfiguration<AlertEntity>
{
    public void Configure(EntityTypeBuilder<AlertEntity> builder)
    {
        builder.ToTable("alert");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.HasKey(e => e.Id);
        
        
        builder.Property(e => e.Message)
            .HasColumnName("message")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(e => e.IssuedAt)
            .HasColumnName("issued_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        
        builder.HasMany(a => a.AlertLocations)
            .WithOne(la => la.Alert)
            .HasForeignKey(al => al.AlertId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}