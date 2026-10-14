using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

public sealed class ForcesCommandDefConfiguration : EffectiveDatedEntityConfiguration<ForcesCommandDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ForcesCommandDef> builder)
    {
        builder.ToTable("ForcesCommandDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
    }
}
