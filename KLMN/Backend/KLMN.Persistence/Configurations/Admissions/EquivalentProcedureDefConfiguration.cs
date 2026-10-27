using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Soft delete/xmin, iki FK, çift yönlü benzersizlik ve self-FK kontrolü.</summary>
public sealed class EquivalentProcedureDefConfiguration:BaseEntityConfiguration<EquivalentProcedureDef>
{
 protected override void ConfigureEntity(EntityTypeBuilder<EquivalentProcedureDef> b)
 {
   b.ToTable("EquivalentProcedureDefs");
   b.HasOne(x=>x.Procedure).WithMany()
    .HasForeignKey(x=>x.ProcedureDefId).OnDelete(DeleteBehavior.Restrict);
   b.HasOne(x=>x.EquivalentProcedure).WithMany()
    .HasForeignKey(x=>x.EquivalentProcedureId).OnDelete(DeleteBehavior.Restrict);
   b.HasIndex(x=>new{x.ProcedureDefId,x.EquivalentProcedureId})
     .IsUnique().HasDatabaseName("UX_EquivalentProcedureDef_Pair")
     .HasFilter("\"IsDeleted\" = false");
   // Guid.CompareTo kanonik alan sırasıyla yazılır; DB düzeyinde self-pair reddi.
   b.ToTable(t=>t.HasCheckConstraint("CK_EquivalentProcedureDefs_Different",
      "\"ProcedureDefId\" <> \"EquivalentProcedureId\""));
 }
}
