using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// UserRole entity EF Core configuration'ıdır.
/// </summary>
internal sealed class UserRoleConfiguration
    : BaseEntityConfiguration<UserRole>
{
    public override void Configure(
        EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserRoles");

        builder.HasIndex(x =>
                new
                {
                    x.UserId,
                    x.RoleId
                })
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
