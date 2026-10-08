using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>User entity EF Core configuration'ıdır.</summary>
public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.NormalizedUserName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.NormalizedEmail).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.Property(x => x.AccessFailedCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.IsLocked).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.TwoFactorAuthenticationEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.LockoutEnd).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ValidFrom).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ValidTo).HasColumnType("timestamp with time zone");
        builder.Property(x => x.LastLoginDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.PasswordChangedDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.SecurityStamp).IsRequired().HasMaxLength(128);

        builder.HasOne(x => x.OrganizationUnit)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.NormalizedUserName)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        builder.HasIndex(x => x.NormalizedEmail)
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        builder.HasIndex(x => x.OrganizationUnitId);
        builder.HasIndex(x => x.ValidTo);
    }
}
