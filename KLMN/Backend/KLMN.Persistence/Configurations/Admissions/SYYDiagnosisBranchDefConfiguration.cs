using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>SYY Gereken Branşlar — Restrict FK, soft-delete koşullu eşsizlik.</summary>
public sealed class SYYDiagnosisBranchDefConfiguration:BaseEntityConfiguration<SYYDiagnosisBranchDef>
{
 protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisBranchDef> b)
 {
  b.ToTable("SYYDiagnosisBranchDefs");
  b.HasOne(x=>x.Diagnosis).WithMany(x=>x.RequiredBranches)
    .HasForeignKey(x=>x.SYYDiagnosisDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.Branch).WithMany()
    .HasForeignKey(x=>x.BranchDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasIndex(x=>new{x.SYYDiagnosisDefId,x.BranchDefId,x.StartingAge,x.EndingAge,x.Gender})
    .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
