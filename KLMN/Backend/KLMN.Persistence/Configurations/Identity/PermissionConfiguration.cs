using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>Permission entity EF Core configuration'ıdır.</summary>
public sealed class PermissionConfiguration : BaseEntityConfiguration<Permission>
{
    /// <summary>
    /// İlgili entity'nin PostgreSQL tablo, sütun, foreign key ve indeks kurallarını uygular.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<Permission> builder)
    {
        // Entity'nin PostgreSQL üzerinde saklanacağı fiziksel tablo adını tanımlar.
        builder.ToTable("Permissions");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Module).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SortOrder).IsRequired().HasDefaultValue(0);

        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => new { x.Module, x.SortOrder });
    }
}
