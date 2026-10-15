using System.Linq.Expressions;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryClassGroups.Responses;

/// <summary>Liste/detay formu için grup, kuvvet ve xmin alanlarını taşır.</summary>
public sealed class MilitaryClassGroupResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = "";
    public string Name { get; init; } = "";
    public string? ShortName { get; init; }
    public Guid ForcesCommandId { get; init; }
    public string ForcesCommandName { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public uint Version { get; init; }
    public DateTime CreatedDate { get; init; }
    /// <summary>EF Core tarafından SQL'e çevrilebilen response izdüşümüdür.</summary>
    public static Expression<Func<MilitaryClassGroupDef, MilitaryClassGroupResponse>> Projection => x =>
        new MilitaryClassGroupResponse {
            Id = x.Id, Code = x.Code, Name = x.Name, ShortName = x.ShortName,
            ForcesCommandId = x.ForcesCommandId, ForcesCommandName = x.ForcesCommand.Name,
            IsActive = x.IsActive, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            Version = x.Version, CreatedDate = x.CreatedDate
        };
}

/// <summary>Değişiklik sonrası kimlik ve PostgreSQL xmin sürümüdür.</summary>
public sealed record MilitaryClassGroupMutationResponse(Guid Id, uint Version);
