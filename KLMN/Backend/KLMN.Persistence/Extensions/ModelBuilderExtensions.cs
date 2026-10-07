using System.Reflection;
using KLMN.Application.Common.Constants;
using KLMN.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Extensions;

/// <summary>
/// KLMN EF Core model oluşturma yardımcılarını içerir.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// BaseEntity türevlerine named global query filter'ları merkezi olarak uygular.
    /// </summary>
    public static void ApplyKLMNGlobalQueryFilters(
        this ModelBuilder modelBuilder)
    {
        var method =
            typeof(ModelBuilderExtensions)
                .GetMethod(
                    nameof(ApplyBaseEntityFilters),
                    BindingFlags.Static |
                    BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                "Global query filter helper bulunamadı.");

        var entityTypes =
            modelBuilder.Model
                .GetEntityTypes()
                .Select(x => x.ClrType)
                .Where(x =>
                    typeof(BaseEntity)
                        .IsAssignableFrom(x))
                .Distinct()
                .ToArray();

        foreach (var entityType in entityTypes)
        {
            method
                .MakeGenericMethod(entityType)
                .Invoke(
                    null,
                    [modelBuilder]);
        }
    }

    private static void ApplyBaseEntityFilters<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(
                QueryFilterNames.SoftDeleteFilter,
                entity => !entity.IsDeleted);

        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(
                QueryFilterNames.ActiveFilter,
                entity => entity.IsActive);
    }
}
