using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class UserConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.HasKey(u => u.UserID);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(u => u.Gender).HasMaxLength(10);
        builder.Property(u => u.RefreshToken).HasMaxLength(500);
        builder.Property(u => u.GoogleId).HasMaxLength(100);
        builder.Property(u => u.RefreshTokenExpiryTime).HasColumnType("datetimeoffset");
        builder.HasMany(u => u.Roles).WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    j => j.HasOne<Roles>().WithMany().HasForeignKey("RoleId"),
                    j => j.HasOne<Users>().WithMany().HasForeignKey("UserId"),
                    j =>{
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                    });
        builder.ToTable("Users");
    }
}
