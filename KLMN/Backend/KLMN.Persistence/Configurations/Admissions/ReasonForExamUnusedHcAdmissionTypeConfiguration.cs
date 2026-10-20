using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Kullanılmayacak Hasta Grupları için Restrict FK ve soft-delete filtreli benzersiz eşleştirme anahtarı.</summary>
public sealed class ReasonForExamUnusedHcAdmissionTypeConfiguration : BaseEntityConfiguration<ReasonForExamUnusedHcAdmissionType>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReasonForExamUnusedHcAdmissionType> b)
    {
        b.ToTable("ReasonForExamUnusedHcAdmissionTypes");
        b.HasOne(x => x.Reason).WithMany(x => x.UnusedAdmissionTypes)
            .HasForeignKey(x => x.ReasonForExaminationDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.AdmissionType).WithMany(x => x.UnusedExaminationReasons)
            .HasForeignKey(x => x.HcAdmissionTypeDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.ReasonForExaminationDefId, x.HcAdmissionTypeDefId })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
