using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Birlikte Verilmeyecek Kararlar için silinmeyen kayıtlar arasında benzersiz çift ve Restrict FK tanımları.</summary>
public sealed class HcUnusedDecisionConfiguration:BaseEntityConfiguration<HcUnusedDecision>
{
 protected override void ConfigureEntity(EntityTypeBuilder<HcUnusedDecision> b)
 {
    b.ToTable("HcUnusedDecisions");
    b.HasOne(x=>x.Decision).WithMany(x=>x.UnusedDecisions).HasForeignKey(x=>x.HcDecisionDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasOne(x=>x.UnusedDecision).WithMany().HasForeignKey(x=>x.UnusedHcDecisionDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasIndex(x=>new{x.HcDecisionDefId,x.UnusedHcDecisionDefId}).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
