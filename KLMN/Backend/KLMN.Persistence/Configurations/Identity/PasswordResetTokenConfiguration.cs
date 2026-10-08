using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>PasswordResetToken entity mapping'ini tanımlar.</summary>
public sealed class PasswordResetTokenConfiguration : BaseEntityConfiguration<PasswordResetToken>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");

        builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
        builder.Property(x => x.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(x => x.UsedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordResetTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);

        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsUsed);
        builder.Ignore(x => x.IsRevoked);
        builder.Ignore(x => x.IsValid);
    }
}
