using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Sağlık tesisi ve rapor türü FK'leri ile benzersiz çift indeksini tanımlar.</summary>
public sealed class HospitalReportTypeConfiguration
    : BaseEntityConfiguration<HospitalReportType>
{
    /// <summary>Eşleştirme yalnız iki FK içerir, soft-delete sonrası çift tekrar kullanılabilir.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<HospitalReportType> builder)
    {
        builder.ToTable("HospitalReportTypes");
        builder.HasIndex(x => new { x.HospitalDefId, x.ReportTypeDefId })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasOne(x => x.Hospital).WithMany(x => x.ReportTypes)
            .HasForeignKey(x => x.HospitalDefId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReportType).WithMany(x => x.Hospitals)
            .HasForeignKey(x => x.ReportTypeDefId).OnDelete(DeleteBehavior.Restrict);
    }
}
