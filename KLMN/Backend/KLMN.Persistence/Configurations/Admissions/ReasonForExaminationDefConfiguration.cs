using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Muayene sebebi ve geçerlilik tarihlerini veritabanına eşler.</summary>
public sealed class ReasonForExaminationDefConfiguration : EffectiveDatedEntityConfiguration<ReasonForExaminationDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReasonForExaminationDef> b)
    {
        b.ToTable("ReasonForExaminationDefs");
        b.Property(x => x.Code).IsRequired().HasMaxLength(100);
        b.Property(x => x.Description).IsRequired().HasMaxLength(500);
        b.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
