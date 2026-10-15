using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

/// <summary>
/// Askerî okul geçerlilik alanlarını ortak mapping'den, şehir/kuvvet
/// bağlantılarını ise nullable Restrict FK'lerden yapılandırır.
/// </summary>
public sealed class MilitarySchoolDefConfiguration : EffectiveDatedEntityConfiguration<MilitarySchoolDef>
{
    /// <summary>Tablo, benzersiz kod ve iki üst tanım ilişkisini eşler.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<MilitarySchoolDef> builder)
    {
        builder.ToTable("MilitarySchoolDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CityId);
        builder.HasIndex(x => x.ForcesCommandDefId);
        builder.HasOne(x => x.City).WithMany(x => x.MilitarySchools)
            .HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ForcesCommand).WithMany(x => x.MilitarySchools)
            .HasForeignKey(x => x.ForcesCommandDefId).OnDelete(DeleteBehavior.Restrict);
    }
}
