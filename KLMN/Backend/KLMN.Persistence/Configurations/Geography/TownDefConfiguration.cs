using KLMN.Domain.Entities.Geography;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Geography;

/// <summary>
/// İlçe sütunlarını, silinmemiş kayıtlarda benzersiz kodu ve şehir bağlantısını
/// aynı BaseEntity/PostgreSQL konvansiyonlarıyla yapılandırır.
/// </summary>
public sealed class TownDefConfiguration : BaseEntityConfiguration<TownDef>
{
    /// <summary>EF Core tablosu, indeksleri ve FK Restrict ilişkisini tanımlar.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<TownDef> builder)
    {
        builder.ToTable("TownDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ExternalCode).HasMaxLength(100);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CityId);
        builder.HasOne(x => x.City).WithMany(x => x.Towns)
            .HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
    }
}
