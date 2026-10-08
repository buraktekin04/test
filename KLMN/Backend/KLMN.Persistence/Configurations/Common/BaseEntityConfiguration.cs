using KLMN.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Common;

/// <summary>BaseEntity türevlerinin ortak EF Core mapping kurallarını tanımlar.</summary>
public abstract class BaseEntityConfiguration<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureBaseEntity(builder);
        ConfigureEntity(builder);
    }

    protected virtual void ConfigureBaseEntity(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.UpdatedBy);

        builder.Property(x => x.DeletedBy);

        builder.Property(x => x.UpdatedDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.DeletedDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Npgsql uint + IsRowVersion() => PostgreSQL xmin.
        builder.Property(x => x.Version)
            .IsRowVersion();
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}

