using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryOffices.Responses;

/// <summary>Şube listesi/detayı: yalnız CityDef ilişkisi, RelatedCity yoktur.</summary>
public sealed class MilitaryOfficeDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ExternalCode { get; init; }
    public Guid? CityId { get; init; }
    public string? CityName { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>Liste/detay ekranları için SQL uyumlu projeksiyon.</summary>

}
