using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>UserPermission override mapping'ini tanımlar.</summary>
public sealed class UserPermissionConfiguration : BaseEntityConfiguration<UserPermission>
{
    /// <summary>
    /// Entity'nin tablo, sütun tipi, foreign key, index ve silme davranışlarını yapılandırır.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<UserPermission> builder)
    {
        // EF Core entity'sinin PostgreSQL'de saklanacağı tablo adını belirler.
        builder.ToTable("UserPermissions");

        builder.Property(x => x.IsGranted).IsRequired();

        // Navigation, foreign key ve bağlı kayıt silme davranışını eşler.
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation, foreign key ve bağlı kayıt silme davranışını eşler.
        builder.HasOne(x => x.Permission)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => new { x.UserId, x.PermissionId })
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.PermissionId);
    }
}
