using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Bağlı kurul kararını ortak audit/soft-delete/UTC süre konvansiyonuna eşler.</summary>
public sealed class HcConnectedDecisionDefConfiguration
    : EffectiveDatedEntityConfiguration<HcConnectedDecisionDef>
{
    /// <summary>Yalnız Code/Name alanlarını ve silinmemiş kod tekilliğini kurar.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<HcConnectedDecisionDef> builder)
    {
        builder.ToTable("HcConnectedDecisionDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
    }
}
