using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Provizyon ilişkileri, durum referansı ve PostgreSQL concurrency kuralları.</summary>
public sealed class ProvisionConfiguration : BaseEntityConfiguration<Provision>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Provision> b)
    {
        b.ToTable("Provisions", t =>
            t.HasCheckConstraint("CK_Provision_WithoutDispatch","\"WithoutDispatch\" IS NULL OR \"WithoutDispatch\" IN (0,1)"));
        b.Property(x => x.ProvisionNo).UseIdentityByDefaultColumn();
        b.HasIndex(x => x.ProvisionNo).IsUnique();
        b.HasIndex(x => x.DispatchId).IsUnique().HasFilter("\"IsDeleted\" = false");
        b.HasIndex(x => new { x.DispatchHospitalId,x.ProvisionDate });
        b.Property(x => x.ProvisionDate).HasColumnType("timestamp with time zone");
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.Photo).HasColumnType("bytea");
        b.HasOne<ProvisionStatusDef>().WithMany()
            .HasForeignKey(x => x.CurrentState)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Dispatch).WithMany().HasForeignKey(x => x.DispatchId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.DispatchHospital).WithMany().HasForeignKey(x => x.DispatchHospitalId).OnDelete(DeleteBehavior.Restrict);
    }
}
