using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Sağlık Tesisinı ortak audit/soft-delete/UTC süre konvansiyonuna eşler.</summary>
public sealed class HospitalDefConfiguration
    : EffectiveDatedEntityConfiguration<HospitalDef>
{
    /// <summary>Yalnız Code/Name alanlarını ve silinmemiş kod tekilliğini kurar.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<HospitalDef> builder)
    {
        builder.ToTable("HospitalDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CityId);
        builder.HasIndex(x => x.RefereeHospitalId);
        builder.HasOne(x => x.City).WithMany(x => x.Hospitals)
            .HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RefereeHospital).WithMany(x => x.ReferringHospitals)
            .HasForeignKey(x => x.RefereeHospitalId).OnDelete(DeleteBehavior.Restrict);
    }
}
