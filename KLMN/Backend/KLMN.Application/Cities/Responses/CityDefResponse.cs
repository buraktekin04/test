using System.Linq.Expressions;
using KLMN.Domain.Entities.Geography;

namespace KLMN.Application.Cities.Responses;

public sealed class CityDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ExternalCode { get; init; }
    public Guid? CountryId { get; init; }
    public string? CountryName { get; init; }
    public int? MernisCode { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    public static Expression<Func<CityDef, CityDefResponse>> Projection => x =>
        new CityDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode,
            CountryId=x.CountryId, CountryName=x.Country == null ? null : x.Country.Name,
            MernisCode=x.MernisCode, ValidFrom=x.ValidFrom, ValidTo=x.ValidTo,
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        };
}
