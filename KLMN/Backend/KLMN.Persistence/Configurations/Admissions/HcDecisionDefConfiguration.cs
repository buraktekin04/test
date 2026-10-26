using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Sayısal karar kodunu ve geçerlilik dönemini PostgreSQL üzerinde benzersiz tanımlar.</summary>
public sealed class HcDecisionDefConfiguration : EffectiveDatedEntityConfiguration<HcDecisionDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<HcDecisionDef> b)
    {
        b.ToTable("HcDecisionDefs");
        b.Property(x=>x.Name).HasMaxLength(500).IsRequired();
        b.HasIndex(x=>x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
