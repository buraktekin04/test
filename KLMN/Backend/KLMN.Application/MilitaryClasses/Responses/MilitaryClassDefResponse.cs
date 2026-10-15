using System.Linq.Expressions;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryClasses.Responses;

/// <summary>
/// Liste ve detay DTO'sudur. Aktiflik yalnızca BaseEntity.IsActive
/// alanıyla yönetilir; güncel xmin Version alanını içerir.
/// </summary>
public sealed class MilitaryClassDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ShortName { get; init; }
    public string? ExternalCode { get; init; }
    public bool IsActive { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>EF Core üzerinde sunucuda çalıştırılabilen liste projeksiyonu.</summary>
    public static Expression<Func<MilitaryClassDef, MilitaryClassDefResponse>> Projection => x =>
        new MilitaryClassDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            ShortName = x.ShortName, ExternalCode = x.ExternalCode,
            IsActive = x.IsActive,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy,
            Version = x.Version
        };
}
