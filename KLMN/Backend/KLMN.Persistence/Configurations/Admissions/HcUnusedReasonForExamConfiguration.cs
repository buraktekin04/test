using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Kullanılmayacak Muayene Sebepleri için silinmeyen kayıtlar arasında benzersiz çift ve Restrict FK tanımları.</summary>
public sealed class HcUnusedReasonForExamConfiguration:BaseEntityConfiguration<HcUnusedReasonForExam>
{
 protected override void ConfigureEntity(EntityTypeBuilder<HcUnusedReasonForExam> b)
 {
    b.ToTable("HcUnusedReasonForExams");
    b.HasOne(x=>x.Decision).WithMany(x=>x.UnusedReasons).HasForeignKey(x=>x.HcDecisionDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasOne(x=>x.Reason).WithMany().HasForeignKey(x=>x.ReasonForExaminationDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasIndex(x=>new{x.HcDecisionDefId,x.ReasonForExaminationDefId}).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
