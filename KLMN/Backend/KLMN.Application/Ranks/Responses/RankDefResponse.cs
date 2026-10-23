using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.Ranks.Responses;

/// <summary>Rütbe liste/detay yanıtı.</summary>
public sealed class RankDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ShortName { get; init; }
    public int Type { get; init; }
    public Guid? RankGroupId { get; init; }
    public string? RankGroupName { get; init; }
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
