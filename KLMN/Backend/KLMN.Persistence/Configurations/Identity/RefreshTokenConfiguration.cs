using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// RefreshToken entity EF Core configuration'ıdır.
/// </summary>
internal sealed class RefreshTokenConfiguration
    : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(
        EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("RefreshTokens");

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.CreatedByIp)
            .HasMaxLength(64);

        builder.Property(x => x.RevokedByIp)
            .HasMaxLength(64);

        builder.Property(x => x.RevocationReason)
            .HasMaxLength(250);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1000);

        builder.Property(x => x.DeviceName)
            .HasMaxLength(200);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x =>
            new
            {
                x.UserId,
                x.ExpiresAt
            });

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReplacedByToken)
            .WithMany()
            .HasForeignKey(x => x.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
