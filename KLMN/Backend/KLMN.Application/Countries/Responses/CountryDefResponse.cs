using KLMN.Domain.Entities.Geography;

namespace KLMN.Application.Countries.Responses;

public sealed class CountryDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ExternalCode { get; init; }
    /// <summary>Harici koddan bağımsız, örneğin TR/DE/USA kısa ülke kodu.</summary>
    public string? ShortCode { get; init; }
    public bool? IsTownRequired { get; init; }
    public int CityCount { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }


}
