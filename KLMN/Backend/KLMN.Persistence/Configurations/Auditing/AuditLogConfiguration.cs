using KLMN.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Auditing;

/// <summary>AuditLogs tablosu, sütun sınırları ve geçmiş sorgusu indekslerini tanımlar.</summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.OccurredAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.ActorUserName).HasMaxLength(150);
        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(80);
        builder.Property(x => x.RelatedEntityType).HasMaxLength(80);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ChangesJson).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.OccurredAt });
        builder.HasIndex(x => new { x.RelatedEntityType, x.RelatedEntityId, x.OccurredAt });
        builder.HasIndex(x => x.OccurredAt);
        builder.HasIndex(x => x.ActorUserId);
    }
}
