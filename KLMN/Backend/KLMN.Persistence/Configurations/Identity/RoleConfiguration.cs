using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>Role entity EF Core configuration'ıdır.</summary>
public sealed class RoleConfiguration : BaseEntityConfiguration<Role>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsSystemRole).IsRequired().HasDefaultValue(false);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
