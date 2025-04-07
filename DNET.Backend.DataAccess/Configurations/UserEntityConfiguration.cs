using DNET.Backend.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNET.Backend.DataAccess.Configurations;

public class UserEntityConfigurations : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("user");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(e => e.Email);

        builder.Property(e => e.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(e => e.PasswordSalt)
            .HasColumnName("password_salt")
            .HasMaxLength(64)
            .IsRequired();
        
        builder.Property(e => e.LoginProvider)
            .HasColumnName("login_provider")
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(e => e.RefreshTokenHash)
            .HasColumnName("refresh_token_hash")
            .HasMaxLength(64);

        builder.Property(e => e.RefreshTokenExpiration)
            .HasColumnName("refresh_token_expiration");
        
        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasMaxLength(32)
            .HasColumnType("character varying(32)")
            .HasDefaultValue("User")
            .IsRequired();

        builder.Property(u => u.PasswordResetToken)
            .HasColumnName("password_reset_token")
            .HasMaxLength(64)
            .HasColumnType("character varying(64)");

        builder.Property(u => u.PasswordResetTokenExpires)
            .HasColumnName("password_reset_token_expires")
            .HasColumnType("timestamp with time zone");
        
        builder.Property(e => e.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(30);

        builder.Property(e => e.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(256);
    }
}