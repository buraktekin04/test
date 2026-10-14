using System.Linq.Expressions;
using KLMN.Domain.Entities.Geography;

namespace KLMN.Application.Countries.Responses;

public sealed class CountryDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ExternalCode { get; init; }
    public bool? IsTownRequired { get; init; }
    public int CityCount { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    public static Expression<Func<CountryDef, CountryDefResponse>> Projection => x =>
        new CountryDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode,
            IsTownRequired=x.IsTownRequired, CityCount=x.Cities.Count(c => !c.IsDeleted),
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        };
}
