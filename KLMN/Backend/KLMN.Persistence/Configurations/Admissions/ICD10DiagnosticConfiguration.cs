using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Üç mevcut tanım arasındaki istatistik ilişkisinin FK/tekillik kuralları.</summary>
public sealed class ICD10DiagnosticConfiguration:BaseEntityConfiguration<ICD10Diagnostic>
{
 protected override void ConfigureEntity(EntityTypeBuilder<ICD10Diagnostic> b)
 {
  b.ToTable("ICD10Diagnostics");
  b.HasOne(x=>x.ICD10).WithMany(x=>x.Diagnostics)
   .HasForeignKey(x=>x.ICD10DefId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.SYYDiagnosis).WithMany(x=>x.ICD10Diagnostics)
   .HasForeignKey(x=>x.SYYDiagnosisDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.AdmissionType).WithMany(x=>x.ICD10Diagnostics)
   .HasForeignKey(x=>x.HcAdmissionTypeDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasIndex(x=>new{x.ICD10DefId,x.SYYDiagnosisDefId,x.HcAdmissionTypeDefId})
   .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
  b.ToTable(t=>t.HasCheckConstraint("CK_ICD10Diagnostics_Amount_Nonnegative","\"Amount\" >= 0"));
 }
}
