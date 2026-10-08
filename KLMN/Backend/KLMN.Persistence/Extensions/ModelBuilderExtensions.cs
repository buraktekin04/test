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
        // Named global query filter uygulanabilecek BaseEntity kök tipleridir.
        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(IsBaseEntityRootType)
            .ToList();

        foreach (var entityType in entityTypes)
        {
            // IsDeleted=false koşulunu sağlayan LINQ expression filtresidir.
            var softDeleteFilter = CreateBooleanFilter(
                entityType.ClrType,
                nameof(BaseEntity.IsDeleted),
                false);

            // IsActive=true koşulunu sağlayan LINQ expression filtresidir.
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

    /// <summary>
    /// Entity'nin filtre uygulanabilen BaseEntity kök tipi olup olmadığını belirler.
    /// </summary>
    private static bool IsBaseEntityRootType(IReadOnlyEntityType entityType) =>
        typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) &&
        entityType.BaseType is null;

    /// <summary>
    /// Entity özelliğini beklenen boolean değerle karşılaştıran LINQ expression oluşturur.
    /// </summary>
    private static LambdaExpression CreateBooleanFilter(
        Type entityType,
        string propertyName,
        bool expectedValue)
    {
        // Dinamik expression ağacındaki entity parametresidir.
        var parameter = Expression.Parameter(entityType, "entity");
        // Filtrelenen entity'nin boolean property ifadesidir.
        var property = Expression.Property(parameter, propertyName);
        // Filtrenin karşılaştıracağı beklenen boolean sabittir.
        var expected = Expression.Constant(expectedValue);
        // Expression içinde özellik ile beklenen değeri karşılaştıran koşuldur.
        var body = Expression.Equal(property, expected);
        return Expression.Lambda(body, parameter);
    }
}
