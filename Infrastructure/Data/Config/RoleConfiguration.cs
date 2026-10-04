using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class RoleConfiguration : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
        builder.HasMany(r => r.Users).WithMany(u => u.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                j => j.HasOne<Users>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<Roles>().WithMany().HasForeignKey("RoleId"),
                j =>
                {
                    j.HasKey("UserId", "RoleId");
                    j.ToTable("UserRoles");
            });
        builder.ToTable("Roles");
    }
}
