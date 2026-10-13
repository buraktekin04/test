using System.Linq.Expressions;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryUnits.Responses;

/// <summary>Birlik tanımları liste/detay ekranındaki güvenli birim yanıtıdır.</summary>
public sealed class MilitaryUnitDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ExternalCode { get; init; }
    public Guid? ForcesCommandDef { get; init; }
    public string? ForcesCommandName { get; init; }
    public string? ForcesCommandCode { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public Guid? ConnectedTopUnity { get; init; }
    public string? ConnectedTopUnitName { get; init; }
    public string? ConnectedTopUnitCode { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public uint Version { get; init; }
    public int UserCount { get; init; }
    public int ChildCount { get; init; }

    /// <summary>EF Core'un SQL'e çevirebildiği tekil DTO projeksiyonudur.</summary>
    public static Expression<Func<MilitaryUnitDef, MilitaryUnitDefResponse>> Projection => unit =>
        new MilitaryUnitDefResponse
        {
            Id = unit.Id,
            Code = unit.Code,
            ExternalCode = unit.ExternalCode,
            ForcesCommandDef = unit.ForcesCommandDef,
            ForcesCommandName = unit.ForceCommand != null ? unit.ForceCommand.Name : null,
            ForcesCommandCode = unit.ForceCommand != null ? unit.ForceCommand.Code : null,
            ValidFrom = unit.ValidFrom,
            ValidTo = unit.ValidTo,
            Name = unit.Name,
            ConnectedTopUnity = unit.ConnectedTopUnity,
            ConnectedTopUnitName = unit.ConnectedTopUnit != null
                ? unit.ConnectedTopUnit.Name : null,
            ConnectedTopUnitCode = unit.ConnectedTopUnit != null
                ? unit.ConnectedTopUnit.Code : null,
            IsActive = unit.IsActive,
            CreatedDate = unit.CreatedDate,
            Version = unit.Version,
            UserCount = unit.Users.Count(x => !x.IsDeleted),
            ChildCount = unit.Children.Count(x => !x.IsDeleted)
        };
}
