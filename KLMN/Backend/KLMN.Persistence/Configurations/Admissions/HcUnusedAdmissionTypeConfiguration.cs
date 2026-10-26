using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Kullanılmayacak Başvuru Grupları için silinmeyen kayıtlar arasında benzersiz çift ve Restrict FK tanımları.</summary>
public sealed class HcUnusedAdmissionTypeConfiguration:BaseEntityConfiguration<HcUnusedAdmissionType>
{
 protected override void ConfigureEntity(EntityTypeBuilder<HcUnusedAdmissionType> b)
 {
    b.ToTable("HcUnusedAdmissionTypes");
    b.HasOne(x=>x.Decision).WithMany(x=>x.UnusedAdmissionTypes).HasForeignKey(x=>x.HcDecisionDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasOne(x=>x.AdmissionType).WithMany().HasForeignKey(x=>x.HcAdmissionTypeDefId).OnDelete(DeleteBehavior.Restrict);
    b.HasIndex(x=>new{x.HcDecisionDefId,x.HcAdmissionTypeDefId}).IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
 }
}
