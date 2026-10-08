using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>RefreshToken güvenlik, rotation ve index mapping'ini tanımlar.</summary>
public sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    /// <summary>
    /// İlgili entity'nin PostgreSQL tablo, sütun, foreign key ve indeks kurallarını uygular.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<RefreshToken> builder)
    {
        // Entity'nin PostgreSQL üzerinde saklanacağı fiziksel tablo adını tanımlar.
        builder.ToTable("RefreshTokens");

        builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
        builder.Property(x => x.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreatedByIp).HasMaxLength(64);
        builder.Property(x => x.RevokedByIp).HasMaxLength(64);
        builder.Property(x => x.RevocationReason).HasMaxLength(500);
        builder.Property(x => x.UserAgent).HasMaxLength(1024);
        builder.Property(x => x.DeviceName).HasMaxLength(200);

        // İlişkili entity için foreign key ve silme davranışını yapılandırır.
        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // İlişkili entity için foreign key ve silme davranışını yapılandırır.
        builder.HasOne(x => x.ReplacedByToken)
            .WithMany()
            .HasForeignKey(x => x.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.TokenHash).IsUnique();
        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.UserId);
        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.ExpiresAt);
        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.ReplacedByTokenId).IsUnique();

        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsExpired);
        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsRevoked);
        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsValid);
    }
}
