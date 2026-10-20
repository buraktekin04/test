using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Gereken Tetkikler için Restrict FK ve soft-delete filtreli benzersiz eşleştirme anahtarı.</summary>
public sealed class ReasonForExamProcedureDefConfiguration : BaseEntityConfiguration<ReasonForExamProcedureDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReasonForExamProcedureDef> b)
    {
        b.ToTable("ReasonForExamProcedureDefs");
        b.HasOne(x => x.Reason).WithMany(x => x.Procedures)
            .HasForeignKey(x => x.ReasonForExaminationDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Procedure).WithMany(x => x.ExaminationReasons)
            .HasForeignKey(x => x.ProcedureDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.ReasonForExaminationDefId, x.ProcedureDefId, x.StartingAge, x.EndingAge, x.Gender })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
    }
}
