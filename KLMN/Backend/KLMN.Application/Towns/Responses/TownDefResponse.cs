using System.Linq.Expressions;
using KLMN.Domain.Entities.Geography;

namespace KLMN.Application.Towns.Responses;

/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed class TownDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ExternalCode { get; init; }
    public Guid? CityId { get; init; }
    public string? CityName { get; init; }
    public int? MernisCode { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    public static Expression<Func<TownDef, TownDefResponse>> Projection => x =>
        new TownDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode,
            CityId=x.CityId, CityName=x.City == null ? null : x.City.Name,
            MernisCode=x.MernisCode,
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        };
}
