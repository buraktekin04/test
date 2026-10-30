using KLMN.Domain.Entities.Admissions;
using KLMN.Domain.Entities.Geography;
using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Sevk FKs, sıra numarası ve kapsam doğruluğunu PostgreSQL seviyesinde sınırlar.</summary>
public sealed class DispatchConfiguration : BaseEntityConfiguration<Dispatch>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Dispatch> b)
    {
        b.ToTable("Dispatches", t =>
        {
            t.HasCheckConstraint("CK_Dispatch_Scope", "\"DispatchScope\" BETWEEN 1 AND 3");
            t.HasCheckConstraint("CK_Dispatch_State", "\"CurrentState\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Dispatch_Sender", "NOT (\"SenderMilitaryUnitDefId\" IS NOT NULL AND \"SenderMilitaryOfficeDefId\" IS NOT NULL)");
        });
        b.Property(x => x.DispatchNo).UseIdentityByDefaultColumn();
        b.HasIndex(x => x.DispatchNo).IsUnique();
        b.Property(x => x.DispatchDate).HasColumnType("timestamp with time zone");
        b.Property(x => x.DispatchDocumentDate).HasColumnType("date");
        b.Property(x => x.LastNominationDate).HasColumnType("date");
        b.Property(x => x.DispatchDocumentNo).HasMaxLength(50);
        b.Property(x => x.RegistryNumber).HasMaxLength(50);
        b.Property(x => x.MilitaryUnitDescription).HasMaxLength(1000);
        b.Property(x => x.SenderChairOther).HasMaxLength(1000);
        b.Property(x => x.Description).HasMaxLength(500);
        b.HasOne(x => x.Person).WithMany(x => x.Dispatches).HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<ReasonForExaminationDef>().WithMany().HasForeignKey(x => x.ReasonForExaminationDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<HcAdmissionTypeDef>().WithMany().HasForeignKey(x => x.HcAdmissionTypeDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<CityDef>().WithMany().HasForeignKey(x => x.DispatchScopeCityId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<HospitalDef>().WithMany().HasForeignKey(x => x.DispatchScopeHospitalId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<ForcesCommandDef>().WithMany().HasForeignKey(x => x.ForcesCommandDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<RankDef>().WithMany().HasForeignKey(x => x.RankDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MilitaryClassDef>().WithMany().HasForeignKey(x => x.MilitaryClassDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MilitaryUnitDef>().WithMany().HasForeignKey(x => x.MilitaryUnitDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MilitarySchoolDef>().WithMany().HasForeignKey(x => x.MilitarySchoolDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MilitaryUnitDef>().WithMany().HasForeignKey(x => x.SenderMilitaryUnitDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MilitaryOfficeDef>().WithMany().HasForeignKey(x => x.SenderMilitaryOfficeDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MilitaryOfficeDef>().WithMany().HasForeignKey(x => x.MilitaryOfficeDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<FlyingMissionDef>().WithMany().HasForeignKey(x => x.FlyingMissionDefId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PlaneTypeDef>().WithMany().HasForeignKey(x => x.PlaneTypeDefId).OnDelete(DeleteBehavior.Restrict);
    }
}
