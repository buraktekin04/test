using KLMN.Domain.Entities.Geography;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Geography;

public sealed class CityDefConfiguration : EffectiveDatedEntityConfiguration<CityDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<CityDef> builder)
    {
        builder.ToTable("CityDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ExternalCode).HasMaxLength(100);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CountryId);
        builder.HasOne(x => x.Country).WithMany(x => x.Cities)
            .HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.Restrict);
    }
}
