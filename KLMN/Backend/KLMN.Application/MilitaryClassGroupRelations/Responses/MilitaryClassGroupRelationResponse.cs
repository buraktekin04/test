using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryClassGroupRelations.Responses;

/// <summary>Kuvvet, askerî sınıf ve grup adlarıyla okunabilir eşleştirme DTO'su.</summary>
public sealed class MilitaryClassGroupRelationResponse
{
    public Guid Id { get; init; }
    public Guid ForcesCommandId { get; init; }
    public string ForcesCommandName { get; init; } = "";
    public Guid MilitaryClassId { get; init; }
    public string MilitaryClassName { get; init; } = "";
    public Guid MilitaryClassGroupId { get; init; }
    public string MilitaryClassGroupName { get; init; } = "";
    public bool IsActive { get; init; }
    public uint Version { get; init; }
    public DateTime CreatedDate { get; init; }

    /// <summary>Liste/Excel için EF Core tarafından SQL'e çevrilebilen projeksiyon.</summary>

}
