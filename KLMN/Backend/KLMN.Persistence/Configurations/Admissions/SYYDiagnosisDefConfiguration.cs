using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>SYY Tanı Listesi için self FK, dinamik tanı türü ve dilim bağlantıları.</summary>
public sealed class SYYDiagnosisDefConfiguration:EffectiveDatedEntityConfiguration<SYYDiagnosisDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisDef> b)
    {
        b.ToTable("SYYDiagnosisDefs");
        b.Property(x=>x.Code).HasMaxLength(100).IsRequired();
        b.Property(x=>x.Name).HasMaxLength(500).IsRequired();
        b.HasIndex(x=>x.Code).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        b.HasIndex(x=>x.SYYDiagnosisTypeDefId);
        b.HasIndex(x=>x.SYYDiagnosisSliceDefId);
        b.HasOne(x=>x.DiagnosisTypeDef).WithMany(x=>x.Diagnoses)
         .HasForeignKey(x=>x.SYYDiagnosisTypeDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.SliceDef).WithMany(x=>x.Diagnoses)
         .HasForeignKey(x=>x.SYYDiagnosisSliceDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.AttachedDiagnosis).WithMany(x=>x.AttachedChildren)
         .HasForeignKey(x=>x.AttachedRecNo).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x=>x.AttachedRecNo);
    }
}
