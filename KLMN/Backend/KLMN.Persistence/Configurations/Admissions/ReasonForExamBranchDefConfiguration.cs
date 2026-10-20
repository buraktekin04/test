using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Muayene Gereken Branşlar için Restrict FK ve soft-delete filtreli benzersiz eşleştirme anahtarı.</summary>
public sealed class ReasonForExamBranchDefConfiguration : BaseEntityConfiguration<ReasonForExamBranchDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReasonForExamBranchDef> b)
    {
        b.ToTable("ReasonForExamBranchDefs");
        b.HasOne(x => x.Reason).WithMany(x => x.Branches)
            .HasForeignKey(x => x.ReasonForExaminationDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Branch).WithMany(x => x.ExaminationReasons)
            .HasForeignKey(x => x.BranchDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.ReasonForExaminationDefId, x.BranchDefId, x.StartingAge, x.EndingAge, x.Gender })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
