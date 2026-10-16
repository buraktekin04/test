using System.Text.Json;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KLMN.Persistence.Interceptors;

/// <summary>
/// BaseEntity audit alanlarını/soft-delete'ı düzenler ve aynı SaveChanges
/// transaction'ında gizli değer içermeyen değişiklik geçmişini yazar.
/// </summary>
public sealed class AuditableEntitySaveChangesInterceptor(
    ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    /*
     * Varsayılan olarak hiçbir entity kaydedilmez. Yalnızca tanımlama türleri
     * ve açıkça izin verilen property'ler audit'e alınır. Yeni şifre veya
     * token property'leri eklense dahi kendiliğinden loga düşmez.
     */
    private static readonly IReadOnlyDictionary<string, HashSet<string>> AllowedFields =
        new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
        {
            ["User"] = Fields("UserName", "FirstName", "LastName", "Email",
                "PhoneNumber", "MilitaryUnitDefId", "ClockLimit", "IsLocked",
                "IsActive", "ValidFrom", "ValidTo"),
            ["Role"] = Fields("Name", "Code", "Description", "IsActive"),
            ["ForcesCommandDef"] = Fields("Name", "Code", "IsActive", "ValidFrom", "ValidTo"),
            ["HcAdmissionTypeDef"] = Fields("Code", "Name",
                "ForcesCommandRequired",
                "RankRequired",
                "MilitaryClassRequired",
                "MilitaryUnitRequired",
                "SenderChairRequired",
                "MilitaryOfficeRequired",
                "RegistryNumberRequired",
                "MilitarySchoolRequired",
                "LastNominationDateRequired",
                "HeightWeightRequired",
                "NthAdmissionRequired",
                "DispatchDocumentDateRequired",
                "DispatchDocumentNoRequired",
                "PertemApprovalRequired",
                "ValidFrom", "ValidTo", "IsActive"),
            ["MilitaryOfficeDef"] = Fields("Code", "Name", "ExternalCode", "CityId",
                "ValidFrom", "ValidTo", "IsActive"),
            ["MilitarySchoolDef"] = Fields("Code", "Name", "CityId",
                "ForcesCommandDefId", "ValidFrom", "ValidTo", "IsActive"),
            ["MilitaryClassDef"] = Fields("Code", "Name", "ShortName", "ExternalCode",
                "ValidFrom", "ValidTo", "IsActive"),
            ["MilitaryClassGroupDef"] = Fields("Code", "Name", "ShortName", "ForcesCommandId",
                "ValidFrom", "ValidTo", "IsActive"),
            ["MilitaryClassGroupRelation"] = Fields("ForcesCommandId", "MilitaryClassId",
                "MilitaryClassGroupId", "IsActive"),
            ["CountryDef"] = Fields("Code", "Name", "ShortCode", "ExternalCode", "IsTownRequired", "IsActive"),
            ["CityDef"] = Fields("Code", "Name", "ExternalCode", "CountryId", "MernisCode",
                "ValidFrom", "ValidTo", "IsActive"),
            ["TownDef"] = Fields("Code", "Name", "ExternalCode", "CityId", "MernisCode", "IsActive"),
            ["PurchaseNumberDef"] = Fields("Code", "Name", "ValidFrom", "ValidTo",
                "HealthReportEndDate", "SupplyDescriptionName", "IsActive"),
            ["MilitaryUnitDef"] = Fields("Name", "Code", "ExternalCode", "ForcesCommandDef",
                "ConnectedTopUnity", "IsActive", "ValidFrom", "ValidTo"),
            ["Permission"] = Fields("Name", "Code", "Module",
                "Description", "SortOrder", "IsActive"),
            ["UserRole"] = Fields("UserId", "RoleId", "IsActive"),
            ["RolePermission"] = Fields("RoleId", "PermissionId", "IsActive"),
            ["UserPermission"] = Fields("UserId", "PermissionId", "IsGranted", "IsActive"),
            ["UserAgreementPdf"] = Fields("UserId", "OriginalFileName", "SizeBytes", "IsActive")
        };

    private static HashSet<string> Fields(params string[] values) =>
        new(values, StringComparer.Ordinal);

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null) Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null) Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>Değişiklikleri algılar, audit alanlarını doldurur ve geçmiş satırlarını ekler.</summary>
    private void Apply(DbContext db)
    {
        db.ChangeTracker.DetectChanges();
        var now = DateTime.UtcNow;
        var userId = currentUserService.UserId;
        var userName = currentUserService.UserName;
        var ip = currentUserService.IpAddress;

        // Listeyi önce materyalize ederek yeni audit kayıtlarının aynı turda
        // yeniden işlenmesini veya koleksiyonun değişmesini önleriz.
        var entries = db.ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var state = entry.State;
            var type = entry.Entity.GetType().Name;

            // Önceki değerler, Created/Updated/Deleted alanlarının değiştirilmesinden
            // ÖNCE snapshot olarak saklanır; yalnızca beyaz listedeki alanlar okunur.
            var changes = AllowedFields.TryGetValue(type, out var allowed)
                ? CaptureChanges(entry, state, allowed)
                : null;

            switch (state)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.IsActive = true;
                    entry.Entity.IsDeleted = false;
                    entry.Entity.UpdatedDate = null;
                    entry.Entity.UpdatedBy = null;
                    entry.Entity.DeletedDate = null;
                    entry.Entity.DeletedBy = null;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = now;
                    entry.Entity.UpdatedBy = userId;
                    entry.Property(x => x.CreatedDate).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.IsActive = false;
                    entry.Entity.DeletedDate = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.UpdatedDate = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }

            if (changes is null || changes.Count == 0) continue;

            var action = state switch
            {
                EntityState.Added => "Create",
                EntityState.Deleted => "Delete",
                _ when changes.TryGetValue("IsActive", out var status) &&
                    status.NewValue is bool active =>
                    active ? "Activate" : "Deactivate",
                _ => "Update"
            };

            // İlişki değişikliği, ana kayıt geçmişinde de görüntülenir.
            var (relatedType, relatedId) = GetRelatedEntity(entry);

            db.Set<AuditLog>().Add(new AuditLog
            {
                OccurredAt = now,
                ActorUserId = userId,
                ActorUserName = string.IsNullOrWhiteSpace(userName)
                    ? null : userName[..Math.Min(userName.Length, 150)],
                EntityType = type,
                EntityId = entry.Entity.Id,
                RelatedEntityType = relatedType,
                RelatedEntityId = relatedId,
                Action = action,
                ChangesJson = JsonSerializer.Serialize(changes, JsonSerializerOptions.Web),
                IpAddress = string.IsNullOrWhiteSpace(ip)
                    ? null : ip[..Math.Min(ip.Length, 64)]
            });
        }
    }

    /// <summary>
    /// Yalnızca güvenli, gerçekten değişmiş alanları önce/sonra
    /// karşılaştırması ile kaydeder. Hash, token ve oturum sırları okunmaz.
    /// </summary>
    private static Dictionary<string, AuditValueChange> CaptureChanges(
        EntityEntry<BaseEntity> entry, EntityState state, HashSet<string> allowed)
    {
        var result = new Dictionary<string, AuditValueChange>(StringComparer.Ordinal);

        foreach (var property in entry.Properties)
        {
            if (!allowed.Contains(property.Metadata.Name)) continue;

            var oldValue = state == EntityState.Added ? null : property.OriginalValue;
            var newValue = state == EntityState.Deleted ? null : property.CurrentValue;

            if (state == EntityState.Modified && Equals(oldValue, newValue))
                continue;

            result[property.Metadata.Name] = new AuditValueChange(oldValue, newValue);
        }

        // Sadece izinli alanı olmayan ilişkisel silmelerde dahi tombstone kaydı
        // oluşması için IsDeleted ilişki beyaz listesine eklenmez; Id meta alanındadır.
        return result;
    }

    /// <summary>İlişki kaydını User/Role ana geçmişine bağlayan salt okunur metaveridir.</summary>
    private static (string? RelatedType, Guid? RelatedId) GetRelatedEntity(
        EntityEntry<BaseEntity> entry) => entry.Entity.GetType().Name switch
    {
        "UserRole" or "UserPermission" or "UserAgreementPdf" =>
            ("User", (Guid?)entry.Property("UserId").CurrentValue),
        "RolePermission" =>
            ("Role", (Guid?)entry.Property("RoleId").CurrentValue),
        _ => (null, null)
    };

    private sealed record AuditValueChange(object? OldValue, object? NewValue);
}
