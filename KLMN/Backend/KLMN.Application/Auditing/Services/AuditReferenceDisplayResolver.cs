using System.Text.Json;
using System.Text.Json.Nodes;
using KLMN.Application.Auditing.Responses;
using KLMN.Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Auditing.Services;

/// <summary>
/// Audit verisini değiştirmeden GUID ilişkilerini kullanıcı dostu isimlere
/// dönüştürür. Orijinal ChangesJson kanıt niteliğinde aynen korunur.
/// </summary>
internal static class AuditReferenceDisplayResolver
{
    /// <summary>
    /// Yalnızca izinli ilişki alanları görüntüleme amaçlı çözümlenir;
    /// diğer GUID alanları ve ham audit kayıtları değiştirilmez.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string> FieldTypes =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["RoleId"] = "Role",
            ["PermissionId"] = "Permission",
            ["UserId"] = "User",
            ["ForcesCommandDef"] = "ForcesCommandDef",
            ["MilitaryUnitDefId"] = "MilitaryUnitDef",
            ["OrganizationUnitId"] = "MilitaryUnitDef",
            ["ParentOrganizationUnitId"] = "MilitaryUnitDef",
            ["ParentId"] = "MilitaryUnitDef",
            ["ConnectedTopUnity"] = "MilitaryUnitDef",
            ["RankGroupId"] = "RankGroupDef",
            ["RankGroup"] = "RankGroupDef",
            ["SYYDiagnosisDefId"] = "SYYDiagnosisDef",
            ["MilitaryClassGroup"] = "MilitaryClassGroupDef",
            ["HcDecision"] = "HcConnectedDecisionDef",
            ["SYYDiagnosisRequiredId"] = "SYYDiagnosisRequired",
            ["CityId"] = "CityDef",
            ["RefereeHospitalId"] = "HospitalDef",
            ["HospitalDefId"] = "HospitalDef",
            ["ReasonForExaminationDefId"] = "ReasonForExaminationDef",
            ["HcAdmissionTypeDefId"] = "HcAdmissionTypeDef",
            ["ProcedureDefId"] = "ProcedureDef",
            ["BranchDefId"] = "BranchDef",
            ["AttachedRecNo"] = "SYYDiagnosisDef",
            ["ReportTypeDefId"] = "ReportTypeDef"
        };

    /// <summary>
    /// Sayfadaki bütün ilişki kimliklerini önce toplayıp her entity türü için
    /// tek veritabanı sorgusu yapar. Soft-delete/pasif kayıtlar da aranır.
    /// </summary>
    public static async Task<IReadOnlyCollection<AuditLogResponse>> ResolveAsync(
        IKLMNDbContext dbContext,
        IReadOnlyList<AuditLogResponse> entries,
        CancellationToken cancellationToken)
    {
        if (entries.Count == 0) return entries;

        // Eski ilişki kaldırma hareketlerinde yalnızca Aktiflik değişmişti.
        // Salt okunur ilişki bağlamı bunlar için de gösterilir; audit değiştirilmez.
        var relationContexts = await AuditRelationContextResolver.LoadAsync(
            dbContext, entries, cancellationToken);

        var references = FieldTypes.Values.Distinct(StringComparer.Ordinal)
            .ToDictionary(type => type, _ => new HashSet<Guid>(), StringComparer.Ordinal);

        foreach (var entry in entries)
        {
            if (!TryParse(entry.ChangesJson, out var root)) continue;

            AuditRelationContextResolver.AppendMissingContext(
                entry, root!, relationContexts);

            foreach (var field in root!)
            {
                if (!FieldTypes.TryGetValue(field.Key, out var entityType) ||
                    field.Value is not JsonObject value)
                    continue;

                AddReference(value["oldValue"], references[entityType]);
                AddReference(value["newValue"], references[entityType]);
            }
        }

        // Eski audit hareketlerinde daha sonra silinmiş/pasifleştirilmiş
        // rol ve izinlerin isimleri de mümkün olduğu sürece bulunur.
        var roleIds = references["Role"];
        var permissionIds = references["Permission"];
        var userIds = references["User"];
        var militaryUnitIds = references["MilitaryUnitDef"];
        var rankGroupIds = references["RankGroupDef"];
        var classGroupIds = references["MilitaryClassGroupDef"];
        var hcDecisionIds = references["HcConnectedDecisionDef"];
        var syyRequiredIds = references["SYYDiagnosisRequired"];
        var cityIds = references["CityDef"];
        var hospitalIds = references["HospitalDef"];
        var reportTypeIds = references["ReportTypeDef"];

        var roleLabels = roleIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.Roles.IgnoreQueryFilters().AsNoTracking()
                .Where(x => roleIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id,
                    x => x.Name + " (" + x.Code + ")", cancellationToken);

        var permissionLabels = permissionIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.Permissions.IgnoreQueryFilters().AsNoTracking()
                .Where(x => permissionIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id,
                    x => x.Name + " (" + x.Code + ")", cancellationToken);

        var userLabels = userIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.Users.IgnoreQueryFilters().AsNoTracking()
                .Where(x => userIds.Contains(x.Id))
                .Select(x => new { x.Id, x.UserName, x.FirstName, x.LastName })
                .ToDictionaryAsync(x => x.Id,
                    x => (x.FirstName + " " + x.LastName).Trim() +
                         " (" + x.UserName + ")", cancellationToken);

        var forceIds = references["ForcesCommandDef"];
        var forceLabels = forceIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => forceIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        var militaryUnitLabels = militaryUnitIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => militaryUnitIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id,
                    x => x.Name + " (" + x.Code + ")", cancellationToken);

        var rankGroupLabels = rankGroupIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.RankGroupDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => rankGroupIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        var classGroupLabels = classGroupIds.Count == 0
            ? new Dictionary<Guid,string>()
            : await dbContext.MilitaryClassGroupDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x=>classGroupIds.Contains(x.Id))
                .Select(x=>new{x.Id,x.Name,x.Code})
                .ToDictionaryAsync(x=>x.Id,x=>x.Name+" ("+x.Code+")",cancellationToken);
        var hcDecisionLabels = hcDecisionIds.Count == 0
            ? new Dictionary<Guid,string>()
            : await dbContext.HcConnectedDecisionDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x=>hcDecisionIds.Contains(x.Id))
                .Select(x=>new{x.Id,x.Name,x.Code})
                .ToDictionaryAsync(x=>x.Id,x=>x.Name+" ("+x.Code+")",cancellationToken);
        var syyRequiredLabels = syyRequiredIds.Count == 0
            ? new Dictionary<Guid,string>()
            : await dbContext.SYYDiagnosisRequireds.IgnoreQueryFilters().AsNoTracking()
                .Where(x=>syyRequiredIds.Contains(x.Id))
                .Select(x=>new{x.Id,Name=x.Diagnosis.Name})
                .ToDictionaryAsync(x=>x.Id,x=>x.Name,cancellationToken);

        var cityLabels = cityIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.CityDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => cityIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        var hospitalLabels = hospitalIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.HospitalDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => hospitalIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        var reportTypeLabels = reportTypeIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await dbContext.ReportTypeDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => reportTypeIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        var ReasonForExaminationDefIds = references["ReasonForExaminationDef"];
        var ReasonForExaminationDefLabels = ReasonForExaminationDefIds.Count == 0 ? new Dictionary<Guid,string>() :
            await dbContext.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => ReasonForExaminationDefIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Description, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Description + " (" + x.Code + ")", cancellationToken);

        var HcAdmissionTypeDefIds = references["HcAdmissionTypeDef"];
        var HcAdmissionTypeDefLabels = HcAdmissionTypeDefIds.Count == 0 ? new Dictionary<Guid,string>() :
            await dbContext.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => HcAdmissionTypeDefIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        var ProcedureDefIds = references["ProcedureDef"];
        var ProcedureDefLabels = ProcedureDefIds.Count == 0 ? new Dictionary<Guid,string>() :
            await dbContext.ProcedureDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => ProcedureDefIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.ProcedureCode })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.ProcedureCode + ")", cancellationToken);

        var BranchDefIds = references["BranchDef"];
        var BranchDefLabels = BranchDefIds.Count == 0 ? new Dictionary<Guid,string>() :
            await dbContext.BranchDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x => BranchDefIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Code })
                .ToDictionaryAsync(x => x.Id, x => x.Name + " (" + x.Code + ")", cancellationToken);

        // SYY'nin aynı tablo içindeki bağlı tanısı audit ekranında kod/ad olarak gösterilir.
        var syyIds=references["SYYDiagnosisDef"];
        var syyLabels=syyIds.Count==0?new Dictionary<Guid,string>():
            await dbContext.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
             .Where(x=>syyIds.Contains(x.Id))
             .Select(x=>new {x.Id,x.Code,x.Name})
             .ToDictionaryAsync(x=>x.Id,x=>x.Code+" — "+x.Name,cancellationToken);

        var labels = new Dictionary<string, Dictionary<Guid, string>>(StringComparer.Ordinal)
        {
            ["SYYDiagnosisDef"] = syyLabels,
            ["SYYDiagnosisRequired"] = syyRequiredLabels,
            ["MilitaryClassGroupDef"] = classGroupLabels,
            ["HcConnectedDecisionDef"] = hcDecisionLabels,
            ["Role"] = roleLabels,
            ["Permission"] = permissionLabels,
            ["User"] = userLabels,
            ["ForcesCommandDef"] = forceLabels,
            ["MilitaryUnitDef"] = militaryUnitLabels,
            ["RankGroupDef"] = rankGroupLabels,
            ["CityDef"] = cityLabels,
            ["HospitalDef"] = hospitalLabels,
            ["ReportTypeDef"] = reportTypeLabels,
            ["ReasonForExaminationDef"] = ReasonForExaminationDefLabels,
            ["HcAdmissionTypeDef"] = HcAdmissionTypeDefLabels,
            ["ProcedureDef"] = ProcedureDefLabels,
            ["BranchDef"] = BranchDefLabels
        };

        var results = new List<AuditLogResponse>(entries.Count);
        foreach (var entry in entries)
        {
            if (!TryParse(entry.ChangesJson, out var root))
            {
                results.Add(entry);
                continue;
            }

            AuditRelationContextResolver.AppendMissingContext(
                entry, root!, relationContexts);

            foreach (var field in root!)
            {
                if (!FieldTypes.TryGetValue(field.Key, out var entityType) ||
                    field.Value is not JsonObject value)
                    continue;

                ReplaceReference(value, "oldValue", labels[entityType], entityType);
                ReplaceReference(value, "newValue", labels[entityType], entityType);
            }

            results.Add(entry with { DisplayChangesJson = root.ToJsonString() });
        }

        return results;
    }

    /// <summary>Bozuk/çok eski audit JSON'ları tek sayfalık sorguyu bozmaz.</summary>
    private static bool TryParse(string json, out JsonObject? root)
    {
        root = null;
        try
        {
            root = JsonNode.Parse(json) as JsonObject;
            return root is not null;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>JSON alanındaki GUID'i çözümlenecek kimlikler listesine ekler.</summary>
    private static void AddReference(JsonNode? node, HashSet<Guid> target)
    {
        if (node is JsonValue && Guid.TryParse(node.ToString(), out var id))
            target.Add(id);
    }

    /// <summary>
    /// Eski/yeni değerleri ad (kod) olarak gösterir. Kayıt bulunamıyorsa
    /// teknik GUID yerine anlaşılır bir kayıp kayıt açıklaması gösterilir.
    /// </summary>
    private static void ReplaceReference(
        JsonObject change, string key, IReadOnlyDictionary<Guid, string> lookup,
        string entityType)
    {
        var node = change[key];
        if (node is not JsonValue ||
            !Guid.TryParse(node.ToString(), out var id))
            return;

        if (lookup.TryGetValue(id, out var label))
        {
            change[key] = label;
            return;
        }

        change[key] = entityType switch
        {
            "Role" => "Rol kaydı bulunamadı",
            "Permission" => "Yetki kaydı bulunamadı",
            "User" => "Kullanıcı kaydı bulunamadı",
            "ForcesCommandDef" => "Kuvvet komutanlığı kaydı bulunamadı",
            "MilitaryUnitDef" => "Birlik kaydı bulunamadı",
            "RankGroupDef" => "Rütbe grubu kaydı bulunamadı",
            "CityDef" => "Şehir kaydı bulunamadı",
            "HospitalDef" => "Sağlık tesisi kaydı bulunamadı",
            "ReportTypeDef" => "Rapor türü kaydı bulunamadı",
            _ => "İlişkili kayıt bulunamadı"
        };
    }
}
