using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

/// <summary>Şube tanımı için şehir FK, tekil kod ve ortak geçerlilik alanlarını eşler.</summary>
public sealed class MilitaryOfficeDefConfiguration : EffectiveDatedEntityConfiguration<MilitaryOfficeDef>
{
    /// <summary>Silinmemiş kod tekilliği ve şehir silme kısıtını yapılandırır.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<MilitaryOfficeDef> builder)
    {
        builder.ToTable("MilitaryOfficeDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ExternalCode).HasMaxLength(100);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CityId);
        builder.HasOne(x => x.City).WithMany(x => x.MilitaryOffices)
            .HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
    }
}
