using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// Permission entity EF Core configuration'ıdır.
/// </summary>
internal sealed class PermissionConfiguration
    : BaseEntityConfiguration<Permission>
{
    public override void Configure(
        EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable("Permissions");

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Module)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");
    }
}
