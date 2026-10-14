using KLMN.Domain.Entities.PurchaseNumbers;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.PurchaseNumbers;

/// <summary>Alım numarası EF Core ve PostgreSQL mapping'i.</summary>
public sealed class PurchaseNumberDefConfiguration : EffectiveDatedEntityConfiguration<PurchaseNumberDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PurchaseNumberDef> builder)
    {
        builder.ToTable("PurchaseNumberDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.SupplyDescriptionName).IsRequired().HasMaxLength(500);
        // Nullable ValidFrom/ValidTo, EffectiveDatedEntityConfiguration'da eşlenir.
        builder.Property(x => x.HealthReportEndDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.ValidFrom);
    }
}
