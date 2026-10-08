using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>UserPermission override mapping'ini tanımlar.</summary>
public sealed class UserPermissionConfiguration : BaseEntityConfiguration<UserPermission>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");

        builder.Property(x => x.IsGranted).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.PermissionId })
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        builder.HasIndex(x => x.PermissionId);
    }
}
