using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// RolePermission entity EF Core configuration'ıdır.
/// </summary>
internal sealed class RolePermissionConfiguration
    : BaseEntityConfiguration<RolePermission>
{
    public override void Configure(
        EntityTypeBuilder<RolePermission> builder)
    {
        base.Configure(builder);

        builder.ToTable("RolePermissions");

        builder.HasIndex(x =>
                new
                {
                    x.RoleId,
                    x.PermissionId
                })
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
