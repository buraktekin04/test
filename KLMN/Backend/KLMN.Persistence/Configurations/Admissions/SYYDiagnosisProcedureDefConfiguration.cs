using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>SYY Gereken Tetkikler — Restrict FK, soft-delete koşullu eşsizlik.</summary>
public sealed class SYYDiagnosisProcedureDefConfiguration:BaseEntityConfiguration<SYYDiagnosisProcedureDef>
{
 protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisProcedureDef> b)
 {
  b.ToTable("SYYDiagnosisProcedureDefs");
  b.HasOne(x=>x.Diagnosis).WithMany(x=>x.RequiredProcedures)
    .HasForeignKey(x=>x.SYYDiagnosisDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.Procedure).WithMany()
    .HasForeignKey(x=>x.ProcedureDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasIndex(x=>new{x.SYYDiagnosisDefId,x.ProcedureDefId,x.StartingAge,x.EndingAge,x.Gender})
    .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
