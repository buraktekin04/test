using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// UserPermission entity EF Core configuration'ıdır.
/// </summary>
internal sealed class UserPermissionConfiguration
    : BaseEntityConfiguration<UserPermission>
{
    public override void Configure(
        EntityTypeBuilder<UserPermission> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserPermissions");

        builder.HasIndex(x =>
                new
                {
                    x.UserId,
                    x.PermissionId
                })
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
