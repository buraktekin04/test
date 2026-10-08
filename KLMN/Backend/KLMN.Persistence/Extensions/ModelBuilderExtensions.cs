using System.Linq.Expressions;
using KLMN.Domain.Common;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KLMN.Persistence.Extensions;

/// <summary>EF Core model oluşturma extension'larını içerir.</summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// BaseEntity root tiplerine SoftDeleteFilter ve ActiveFilter named
    /// global query filter'larını uygular.
    /// </summary>
    public static void ApplyGlobalQueryFilters(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(IsBaseEntityRootType)
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var softDeleteFilter = CreateBooleanFilter(
                entityType.ClrType,
                nameof(BaseEntity.IsDeleted),
                false);

            var activeFilter = CreateBooleanFilter(
                entityType.ClrType,
                nameof(BaseEntity.IsActive),
                true);

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(QueryFilterNames.SoftDelete, softDeleteFilter);

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(QueryFilterNames.Active, activeFilter);
        }
    }

    private static bool IsBaseEntityRootType(IReadOnlyEntityType entityType) =>
        typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) &&
        entityType.BaseType is null;

    private static LambdaExpression CreateBooleanFilter(
        Type entityType,
        string propertyName,
        bool expectedValue)
    {
        var parameter = Expression.Parameter(entityType, "entity");
        var property = Expression.Property(parameter, propertyName);
        var expected = Expression.Constant(expectedValue);
        var body = Expression.Equal(property, expected);
        return Expression.Lambda(body, parameter);
    }
}
