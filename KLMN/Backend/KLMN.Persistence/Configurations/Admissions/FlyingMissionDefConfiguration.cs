using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Uçucu Görev Tanımları — ValidFrom/ValidTo, soft delete, xmin ve kod tekilliği.</summary>
public sealed class FlyingMissionDefConfiguration : EffectiveDatedEntityConfiguration<FlyingMissionDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<FlyingMissionDef> b)
    {
        b.ToTable("FlyingMissionDefs");
        b.Property(x=>x.Code).IsRequired().HasMaxLength(100);
        b.Property(x=>x.Name).IsRequired().HasMaxLength(250);
        b.HasIndex(x=>x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
