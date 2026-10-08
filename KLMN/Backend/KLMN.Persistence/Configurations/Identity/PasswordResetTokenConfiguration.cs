using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>PasswordResetToken entity mapping'ini tanımlar.</summary>
public sealed class PasswordResetTokenConfiguration : BaseEntityConfiguration<PasswordResetToken>
{
    /// <summary>
    /// İlgili entity'nin PostgreSQL tablo, sütun, foreign key ve indeks kurallarını uygular.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<PasswordResetToken> builder)
    {
        // Entity'nin PostgreSQL üzerinde saklanacağı fiziksel tablo adını tanımlar.
        builder.ToTable("PasswordResetTokens");

        builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
        builder.Property(x => x.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.UsedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp with time zone");

        // İlişkili entity için foreign key ve silme davranışını yapılandırır.
        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordResetTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.TokenHash).IsUnique();
        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.UserId);
        // Sorgu performansı ve gerekli benzersizlik koşullarını veritabanı indeksinde tanımlar.
        builder.HasIndex(x => x.ExpiresAt);

        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsExpired);
        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsUsed);
        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsRevoked);
        // İlgili entity üzerinde özel EF Core model kuralı uygular.
        builder.Ignore(x => x.IsValid);
    }
}
