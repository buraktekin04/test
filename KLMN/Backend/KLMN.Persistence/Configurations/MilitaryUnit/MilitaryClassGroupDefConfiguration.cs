using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

/// <summary>Grubun ortak UTC dönemini, kuvvet FK'sini ve kuvvet içi benzersiz kodu eşler.</summary>
public sealed class MilitaryClassGroupDefConfiguration : EffectiveDatedEntityConfiguration<MilitaryClassGroupDef>
{
    /// <summary>Soft-delete edilen kodun yeniden tanımlanabilmesi için partial unique index kullanır.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<MilitaryClassGroupDef> builder)
    {
        builder.ToTable("MilitaryClassGroupDefs");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ShortName).HasMaxLength(100);
        builder.HasIndex(x => new { x.ForcesCommandId, x.Code })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.Name);
        builder.HasOne(x => x.ForcesCommand).WithMany(x => x.MilitaryClassGroups)
            .HasForeignKey(x => x.ForcesCommandId).OnDelete(DeleteBehavior.Restrict);
    }
}
