using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Bağlı Kararlar için silinmeyen kayıtlar arasında benzersiz çift ve Restrict FK tanımları.</summary>
public sealed class HcConnectedDecisionConfiguration:BaseEntityConfiguration<HcConnectedDecision>
{
 protected override void ConfigureEntity(EntityTypeBuilder<HcConnectedDecision> b)
 {
    b.ToTable("HcConnectedDecisions");
    b.HasOne(x=>x.Decision).WithMany(x=>x.ConnectedDecisions).HasForeignKey(x=>x.HcDecisionDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasOne(x=>x.ConnectedDecisionDef).WithMany().HasForeignKey(x=>x.HcConnectedDecisionDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasIndex(x=>new{x.HcDecisionDefId,x.HcConnectedDecisionDefId}).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
