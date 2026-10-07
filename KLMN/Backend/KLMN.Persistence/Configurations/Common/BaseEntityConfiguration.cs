using KLMN.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KLMN.Persistence.Configurations.Common;

/// <summary>
/// BaseEntity ortak EF Core configuration alanlarını tanımlar.
/// Query filter'lar burada değil ModelBuilderExtensions içerisinde uygulanır.
/// </summary>
public abstract class BaseEntityConfiguration<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    /// <summary>
    /// Entity'nin ortak alanlarını configure eder.
/// </summary>
    public virtual void Configure(
        EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsRowVersion();
    }
}
