using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Identity;

/// <summary>UserRole ilişki mapping'ini tanımlar.</summary>
public sealed class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
{
    /// <summary>
    /// Entity'nin tablo, sütun tipi, foreign key, index ve silme davranışlarını yapılandırır.
    /// </summary>
    protected override void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
    {
        // EF Core entity'sinin PostgreSQL'de saklanacağı tablo adını belirler.
        builder.ToTable("UserRoles");

        // Navigation, foreign key ve bağlı kayıt silme davranışını eşler.
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation, foreign key ve bağlı kayıt silme davranışını eşler.
        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => new { x.UserId, x.RoleId })
            .IsUnique()
            .HasFilter(PostgreSqlIndexFilters.NotDeleted);

        // Sorgu performansı ve benzersizlik kurallarını destekleyen indeks tanımıdır.
        builder.HasIndex(x => x.RoleId);
    }
}
