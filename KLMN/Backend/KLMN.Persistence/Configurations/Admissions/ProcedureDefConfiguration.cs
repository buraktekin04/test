using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Asgari tetkik referans tablosu; eski ProcedureDef modeli sonradan doğrulanacaktır.</summary>
public sealed class ProcedureDefConfiguration : EffectiveDatedEntityConfiguration<ProcedureDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProcedureDef> b)
    {
        b.ToTable("ProcedureDefs");
        b.Property(x => x.Code).IsRequired().HasMaxLength(100);
        b.Property(x => x.Name).IsRequired().HasMaxLength(250);
        b.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
