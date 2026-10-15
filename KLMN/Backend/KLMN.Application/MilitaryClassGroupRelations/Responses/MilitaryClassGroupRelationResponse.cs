using System.Linq.Expressions;
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
    public static Expression<Func<MilitaryClassGroupRelation, MilitaryClassGroupRelationResponse>> Projection => x =>
        new MilitaryClassGroupRelationResponse {
            Id = x.Id, ForcesCommandId = x.ForcesCommandId, ForcesCommandName = x.ForcesCommand.Name,
            MilitaryClassId = x.MilitaryClassId, MilitaryClassName = x.MilitaryClass.Name,
            MilitaryClassGroupId = x.MilitaryClassGroupId, MilitaryClassGroupName = x.MilitaryClassGroup.Name,
            IsActive = x.IsActive, Version = x.Version, CreatedDate = x.CreatedDate
        };
}

/// <summary>Eşleştirme sonrası Id ve yeni PostgreSQL xmin sürümü.</summary>
public sealed record MilitaryClassGroupRelationMutationResponse(Guid Id, uint Version);
