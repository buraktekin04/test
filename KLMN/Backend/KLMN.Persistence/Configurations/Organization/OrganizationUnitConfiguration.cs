using KLMN.Domain.Entities.Organization;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Organization;

/// <summary>OrganizationUnit hiyerarşi ve index mapping'ini tanımlar.</summary>
public sealed class OrganizationUnitConfiguration : BaseEntityConfiguration<OrganizationUnit>
{
    /// <summary>
    /// Entity'nin tablo, sütun tipi, foreign key, index ve silme davranışlarını yapılandırır.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<OrganizationUnit> builder)
    {
        // EF Core entity'sinin PostgreSQL'de saklanacağı tablo adını belirler.
        builder.ToTable("OrganizationUnits");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        // Navigation, foreign key ve bağlı kayıt silme davranışını eşler.
        builder.HasOne(x => x.ParentOrganizationUnit)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentOrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.ParentOrganizationUnitId);
        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.Name);
    }
}
