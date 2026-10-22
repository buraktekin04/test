using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>SYY Çizelgesi — mevcut beş tanım kaydı için FK ve tarihli satır eşsizlik kuralı.</summary>
public sealed class SYYClassificationDefConfiguration:EffectiveDatedEntityConfiguration<SYYClassificationDef>
{
 protected override void ConfigureEntity(EntityTypeBuilder<SYYClassificationDef> b)
 {
  b.ToTable("SYYClassificationDefs");
  b.HasOne(x=>x.Diagnosis).WithMany(x=>x.Classifications)
     .HasForeignKey(x=>x.SYYDiagnosisDefId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.ForcesCommand).WithMany()
     .HasForeignKey(x=>x.ForcesCommandDef).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.RankGroupDefinition).WithMany()
     .HasForeignKey(x=>x.RankGroup).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.MilitaryClassGroupDefinition).WithMany()
     .HasForeignKey(x=>x.MilitaryClassGroup).OnDelete(DeleteBehavior.Restrict);
  b.HasOne(x=>x.Decision).WithMany()
     .HasForeignKey(x=>x.HcDecision).OnDelete(DeleteBehavior.Restrict);
  b.HasIndex(x=>new{x.SYYDiagnosisDefId,x.ForcesCommandDef,x.RankGroup,x.MilitaryClassGroup,x.HcDecision,x.ValidFrom})
    .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
