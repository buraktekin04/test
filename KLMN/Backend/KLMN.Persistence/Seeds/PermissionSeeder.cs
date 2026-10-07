using System.Reflection;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Seeds;

/// <summary>
/// PermissionCodes sınıfındaki permission tanımlarını veritabanına seed eder.
/// </summary>
internal sealed class PermissionSeeder(
    IKLMNDbContext dbContext)
{
    /// <summary>
    /// Eksik permission kayıtlarını ekler.
/// </summary>
    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var definitions =
            GetPermissionDefinitions();

        var existingCodes =
            await dbContext.Permissions
                .IgnoreQueryFilters()
                .Select(x => x.Code)
                .ToListAsync(cancellationToken);

        var existingSet =
            existingCodes.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var sortOrder = 1;

        foreach (var definition in definitions)
        {
            if (existingSet.Contains(definition.Code))
            {
                sortOrder++;
                continue;
            }

            dbContext.Permissions.Add(
                new Permission
                {
                    Name = definition.Code,
                    Code = definition.Code,
                    Module = definition.Module,
                    SortOrder = sortOrder++
                });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyCollection<
        (string Module, string Code)>
        GetPermissionDefinitions()
    {
        var result =
            new List<(string Module, string Code)>();

        var nestedTypes =
            typeof(PermissionCodes)
                .GetNestedTypes(
                    BindingFlags.Public);

        foreach (var nestedType in nestedTypes)
        {
            var module = nestedType.Name;

            var fields =
                nestedType.GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (!field.IsLiteral ||
                    field.FieldType != typeof(string))
                {
                    continue;
                }

                if (field.GetRawConstantValue() is string code)
                {
                    result.Add((module, code));
                }
            }
        }

        return result
            .OrderBy(x => x.Module)
            .ThenBy(x => x.Code)
            .ToArray();
    }
}
