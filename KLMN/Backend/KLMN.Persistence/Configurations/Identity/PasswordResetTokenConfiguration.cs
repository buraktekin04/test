using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// PasswordResetToken entity EF Core configuration'ıdır.
/// </summary>
internal sealed class PasswordResetTokenConfiguration
    : BaseEntityConfiguration<PasswordResetToken>
{
    public override void Configure(
        EntityTypeBuilder<PasswordResetToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("PasswordResetTokens");

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordResetTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
