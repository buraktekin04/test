using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitarySchools.Responses;

/// <summary>
/// Askerî okulun liste ve detay DTO'sudur. Kod ve ad yanında iki FK'nin
/// görünen adını, UTC geçerliliğini ve optimistik concurrency sürümünü taşır.
/// </summary>
public sealed class MilitarySchoolDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? CityId { get; init; }
    public string? CityName { get; init; }
    public Guid? ForcesCommandDefId { get; init; }
    public string? ForcesCommandName { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>EF Core tarafından SQL'e çevrilebilen server-side DTO projeksiyonudur.</summary>

}
