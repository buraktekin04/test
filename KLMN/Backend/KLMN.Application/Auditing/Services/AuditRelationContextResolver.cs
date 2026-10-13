using System.Text.Json.Nodes;
using KLMN.Application.Auditing.Responses;
using KLMN.Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Auditing.Services;

/// <summary>
/// Audit geçmişinde ilişkiler pasifleştirildiğinde Rol/Yetki/Kullanıcı
/// kimliklerini salt-okunur görüntü bağlamı olarak tamamlar.
/// AuditLogs.ChangesJson veya eski log satırları değiştirilmez.
/// </summary>
internal static class AuditRelationContextResolver
{
    /// <summary>
    /// İlgili RolePermission, UserRole ve UserPermission kayıtlarını
    /// pasif/soft-delete dahil teker toplu sorguyla getirir.
    /// </summary>
    internal static async Task<IReadOnlyDictionary<(string Type, Guid Id), IReadOnlyDictionary<string, Guid>>>
        LoadAsync(
            IKLMNDbContext dbContext,
            IReadOnlyList<AuditLogResponse> entries,
            CancellationToken cancellationToken)
    {
        var result = new Dictionary<(string Type, Guid Id), IReadOnlyDictionary<string, Guid>>();

        var rolePermissionIds = entries
            .Where(x => x.EntityType == "RolePermission")
            .Select(x => x.EntityId).Distinct().ToArray();

        if (rolePermissionIds.Length > 0)
        {
            var relations = await dbContext.RolePermissions.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => rolePermissionIds.Contains(x.Id))
                .Select(x => new { x.Id, x.RoleId, x.PermissionId })
                .ToListAsync(cancellationToken);

            foreach (var relation in relations)
                result[("RolePermission", relation.Id)] = new Dictionary<string, Guid>
                {
                    ["RoleId"] = relation.RoleId,
                    ["PermissionId"] = relation.PermissionId
                };
        }

        var userRoleIds = entries
            .Where(x => x.EntityType == "UserRole")
            .Select(x => x.EntityId).Distinct().ToArray();

        if (userRoleIds.Length > 0)
        {
            var relations = await dbContext.UserRoles.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => userRoleIds.Contains(x.Id))
                .Select(x => new { x.Id, x.UserId, x.RoleId })
                .ToListAsync(cancellationToken);

            foreach (var relation in relations)
                result[("UserRole", relation.Id)] = new Dictionary<string, Guid>
                {
                    ["UserId"] = relation.UserId,
                    ["RoleId"] = relation.RoleId
                };
        }

        var userPermissionIds = entries
            .Where(x => x.EntityType == "UserPermission")
            .Select(x => x.EntityId).Distinct().ToArray();

        if (userPermissionIds.Length > 0)
        {
            var relations = await dbContext.UserPermissions.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => userPermissionIds.Contains(x.Id))
                .Select(x => new { x.Id, x.UserId, x.PermissionId })
                .ToListAsync(cancellationToken);

            foreach (var relation in relations)
                result[("UserPermission", relation.Id)] = new Dictionary<string, Guid>
                {
                    ["UserId"] = relation.UserId,
                    ["PermissionId"] = relation.PermissionId
                };
        }

        return result;
    }

    /// <summary>
    /// Sadece görüntü JSON'una değişmeyen ilişki anahtarlarını ekler.
    /// oldValue == newValue seçimi, bunun bir alan değişikliği DEĞİL
    /// ilişki bağlamı olduğunu frontend'e anlatır.
    /// </summary>
    internal static void AppendMissingContext(
        AuditLogResponse entry,
        JsonObject root,
        IReadOnlyDictionary<(string Type, Guid Id), IReadOnlyDictionary<string, Guid>> contexts)
    {
        if (!contexts.TryGetValue((entry.EntityType, entry.EntityId), out var fields))
            return;

        foreach (var (name, id) in fields)
        {
            // Gerçekte değiştirilmiş, geçmişe yazılmış orijinal değeri ezmeyiz.
            if (root.ContainsKey(name)) continue;
            root[name] = new JsonObject
            {
                ["oldValue"] = id.ToString(),
                ["newValue"] = id.ToString()
            };
        }
    }
}
