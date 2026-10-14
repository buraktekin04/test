using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

/// <summary>MilitaryUnitDef hiyerarşi ve index mapping'ini tanımlar.</summary>
public sealed class MilitaryUnitDefConfiguration : EffectiveDatedEntityConfiguration<MilitaryUnitDef>
{
    /// <summary>
    /// Entity'nin tablo, sütun tipi, foreign key, index ve silme davranışlarını yapılandırır.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<MilitaryUnitDef> builder)
    {
        // EF Core entity'sinin PostgreSQL'de saklanacağı tablo adını belirler.
        builder.ToTable("MilitaryUnitDefs");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ExternalCode).HasMaxLength(100);
        builder.HasIndex(x => x.ForcesCommandDef);
        builder.HasOne(x => x.ForceCommand)
            .WithMany(x => x.MilitaryUnits)
            .HasForeignKey(x => x.ForcesCommandDef)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation, foreign key ve bağlı kayıt silme davranışını eşler.
        builder.HasOne(x => x.ConnectedTopUnit)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ConnectedTopUnity)
            .OnDelete(DeleteBehavior.Restrict);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.ConnectedTopUnity);
        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.Name);
    }
}
