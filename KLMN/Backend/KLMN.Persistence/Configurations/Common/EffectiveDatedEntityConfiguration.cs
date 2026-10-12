using KLMN.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Common;

/// <summary>
/// Geçerlilik dönemi kullanan tanım tablolarına ortak EF Core mapping'i uygular.
/// BaseEntity audit, aktiflik ve xmin konfigürasyonuna dokunmaz.
/// </summary>
/// <typeparam name="TEntity">Geçerlilik tarihleri olan domain entity'sidir.</typeparam>
public abstract class EffectiveDatedEntityConfiguration<TEntity>
    : BaseEntityConfiguration<TEntity>
    where TEntity : EffectiveDatedEntity
{
    /// <summary>
    /// PostgreSQL'de başlangıç/bitiş için UTC destekleyen iki fiziksel
    /// timestamp with time zone kolonu eşler.
    /// </summary>
    protected override void ConfigureBaseEntity(EntityTypeBuilder<TEntity> builder)
    {
        base.ConfigureBaseEntity(builder);

        builder.Property(x => x.ValidFrom)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.ValidTo)
            .HasColumnType("timestamp with time zone");
    }
}
