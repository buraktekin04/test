using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Karar istatistiğinin FK, tekillik ve negatif olmayan miktar kuralları.</summary>
public sealed class SYYDiagnosisDecisionConfiguration : BaseEntityConfiguration<SYYDiagnosisDecision>
{
 protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisDecision> b)
 {
   b.ToTable("SYYDiagnosisDecisions");
   b.HasOne(x=>x.SYYDiagnosis).WithMany().HasForeignKey(x=>x.SYYDiagnosisDefId).OnDelete(DeleteBehavior.Restrict);
   b.HasOne(x=>x.HcDecision).WithMany().HasForeignKey(x=>x.HcDecisionDefId).OnDelete(DeleteBehavior.Restrict);
   b.HasOne(x=>x.AdmissionType).WithMany().HasForeignKey(x=>x.HcAdmissionTypeDefId).OnDelete(DeleteBehavior.Restrict);
   b.HasOne(x=>x.MilitaryClass).WithMany().HasForeignKey(x=>x.MilitaryClassDefId).OnDelete(DeleteBehavior.Restrict);
   // PostgreSQL NULL değerlerini benzersiz saymadığından sınıfsız eşleşmeler için ayrıca kısmi indeks vardır.
   b.HasIndex(x=>new{x.SYYDiagnosisDefId,x.HcDecisionDefId,x.HcAdmissionTypeDefId,x.MilitaryClassDefId})
     .IsUnique().HasDatabaseName("UX_SYYDecision_WithClass")
     .HasFilter("\"IsDeleted\" = false AND \"MilitaryClassDefId\" IS NOT NULL");
   b.HasIndex(x=>new{x.SYYDiagnosisDefId,x.HcDecisionDefId,x.HcAdmissionTypeDefId})
     .IsUnique().HasDatabaseName("UX_SYYDecision_WithoutClass")
     .HasFilter("\"IsDeleted\" = false AND \"MilitaryClassDefId\" IS NULL");
   b.ToTable(t=>t.HasCheckConstraint("CK_SYYDiagnosisDecisions_Amount_Nonnegative","\"Amount\" >= 0"));
 }
}
