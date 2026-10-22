using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Kurala bağlı madde açıklamaları, ayrıştırılmış detay tablosu.</summary>
public sealed class SYYDiagnosisRequiredDetailConfiguration:BaseEntityConfiguration<SYYDiagnosisRequiredDetail>
{
 protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisRequiredDetail> b)
 {
  b.ToTable("SYYDiagnosisRequiredDetails");
  b.Property(x=>x.Description).IsRequired().HasMaxLength(1500);
  b.HasOne(x=>x.Required).WithMany(x=>x.Details)
   .HasForeignKey(x=>x.SYYDiagnosisRequiredId).OnDelete(DeleteBehavior.Restrict);
  // Sıra düzenlenirken önce eklenen satırlar veritabanında geçici çakışma yaratmasın.
  b.HasIndex(x=>new{x.SYYDiagnosisRequiredId,x.SortOrder});
 }
}
