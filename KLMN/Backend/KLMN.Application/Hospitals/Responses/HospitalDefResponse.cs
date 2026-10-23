using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.Hospitals.Responses;

/// <summary>Sağlık tesisi liste/detay yanıtı: şehir ve hakem sağlık tesisi ilişkileriyle birlikte.</summary>
public sealed class HospitalDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? CityId { get; init; }
    public string? CityName { get; init; }
    public Guid? RefereeHospitalId { get; init; }
    public string? RefereeHospitalName { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>EF Core'da sunucu tarafında çevrilebilen DTO projeksiyonu.</summary>

}
