using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Bir SYY tanısına birden çok onay kuralı; her kurala ayrı madde listesi.</summary>
public sealed class SYYDiagnosisRequiredConfiguration:BaseEntityConfiguration<SYYDiagnosisRequired>
{
 protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisRequired> b)
 {
  b.ToTable("SYYDiagnosisRequireds");
  b.HasOne(x=>x.Diagnosis).WithMany(x=>x.RequiredRules)
   .HasForeignKey(x=>x.SYYDiagnosisDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasIndex(x=>x.SYYDiagnosisDefId);
 }
}
