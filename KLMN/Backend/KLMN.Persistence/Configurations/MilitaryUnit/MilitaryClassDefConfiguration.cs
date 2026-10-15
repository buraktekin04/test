using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

/// <summary>Askerî sınıf alanlarını, tekil kod indeksini ve ortak geçerlilik mapping'ini yapılandırır.</summary>
public sealed class MilitaryClassDefConfiguration : EffectiveDatedEntityConfiguration<MilitaryClassDef>
{
    /// <summary>Yalnızca sınıfa özgü kolonları eşler; BaseEntity alanlarını tekrar kurmaz.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<MilitaryClassDef> builder)
    {
        builder.ToTable("MilitaryClassDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ShortName).HasMaxLength(100);
        builder.Property(x => x.ExternalCode).HasMaxLength(100);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
    }
}
