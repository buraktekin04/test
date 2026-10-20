using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Muayene Sebebi Rapor Türleri için Restrict FK ve soft-delete filtreli benzersiz eşleştirme anahtarı.</summary>
public sealed class ReasonForExamReportTypeConfiguration : BaseEntityConfiguration<ReasonForExamReportType>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReasonForExamReportType> b)
    {
        b.ToTable("ReasonForExamReportTypes");
        b.HasOne(x => x.Reason).WithMany(x => x.ReportTypes)
            .HasForeignKey(x => x.ReasonForExaminationDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ReportType).WithMany(x => x.ExaminationReasons)
            .HasForeignKey(x => x.ReportTypeDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.ReasonForExaminationDefId, x.ReportTypeDefId })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
