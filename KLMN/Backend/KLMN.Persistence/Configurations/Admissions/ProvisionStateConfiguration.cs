using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Admissions;

/// <summary>Provizyonun Yeni/Rapor/İptal/Bildirim geçmişi.</summary>
public sealed class ProvisionStateConfiguration : BaseEntityConfiguration<ProvisionState>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProvisionState> b)
    {
        b.ToTable("ProvisionStates", t => t.HasCheckConstraint("CK_ProvisionState_State","\"State\" BETWEEN 1 AND 4"));
        b.HasIndex(x => new {x.ProvisionId,x.CreatedDate});
        b.HasOne(x => x.Provision).WithMany(x => x.States)
            .HasForeignKey(x => x.ProvisionId).OnDelete(DeleteBehavior.Restrict);
    }
}
