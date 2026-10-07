using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>
/// User entity EF Core configuration'ıdır.
/// </summary>
internal sealed class UserConfiguration
    : BaseEntityConfiguration<User>
{
    public override void Configure(
        EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(x => x.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(x => x.SecurityStamp)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(x => x.NormalizedUserName)
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");

        builder.HasIndex(x => x.NormalizedEmail)
            .IsUnique()
            .HasFilter(""IsDeleted" = FALSE");

        builder.HasOne(x => x.OrganizationUnit)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
