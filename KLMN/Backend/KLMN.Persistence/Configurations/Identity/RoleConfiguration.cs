using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>Role entity EF Core configuration'ıdır.</summary>
public sealed class RoleConfiguration : BaseEntityConfiguration<Role>
{
    /// <summary>
    /// İlgili entity'nin PostgreSQL tablo, sütun, foreign key ve indeks kurallarını uygular.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<Role> builder)
    {
        // Entity'nin PostgreSQL üzerinde saklanacağı fiziksel tablo adını tanımlar.
        builder.ToTable("Roles");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsSystemRole).IsRequired().HasDefaultValue(false);

        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
