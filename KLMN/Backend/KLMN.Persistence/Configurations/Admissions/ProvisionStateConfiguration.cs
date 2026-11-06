using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Provizyon durum değişikliklerinin kalıcı tarihçesi.</summary>
public sealed class ProvisionStateConfiguration : BaseEntityConfiguration<ProvisionState>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProvisionState> b)
    {
        b.ToTable("ProvisionStates");
        b.HasOne<ProvisionStatusDef>().WithMany()
            .HasForeignKey(x => x.State)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new {x.ProvisionId,x.CreatedDate});
        b.HasOne(x => x.Provision).WithMany(x => x.States)
            .HasForeignKey(x => x.ProvisionId).OnDelete(DeleteBehavior.Restrict);
    }
}
