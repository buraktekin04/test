using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.MilitaryUnit;

/// <summary>Askerî sınıf-kuvvet-grup üçlü ilişkisini ve aktif kayıt benzersizliğini eşler.</summary>
public sealed class MilitaryClassGroupRelationConfiguration : BaseEntityConfiguration<MilitaryClassGroupRelation>
{
    /// <summary>Üç FK'yi Restrict ve üçlü soft-delete filtreli unique index ile korur.</summary>
    protected override void ConfigureEntity(EntityTypeBuilder<MilitaryClassGroupRelation> builder)
    {
        builder.ToTable("MilitaryClassGroupRelations");
        builder.HasIndex(x => new { x.ForcesCommandId, x.MilitaryClassId, x.MilitaryClassGroupId })
            .IsUnique().HasFilter(PostgreSqlIndexFilters.NotDeleted);
        builder.HasIndex(x => x.MilitaryClassId);
        builder.HasIndex(x => x.MilitaryClassGroupId);
        builder.HasOne(x => x.ForcesCommand).WithMany(x => x.MilitaryClassGroupRelations)
            .HasForeignKey(x => x.ForcesCommandId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MilitaryClass).WithMany(x => x.MilitaryClassGroupRelations)
            .HasForeignKey(x => x.MilitaryClassId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MilitaryClassGroup).WithMany(x => x.Relations)
            .HasForeignKey(x => x.MilitaryClassGroupId).OnDelete(DeleteBehavior.Restrict);
    }
}
