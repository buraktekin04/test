using System.Linq.Expressions;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.ForcesCommands.Responses;

public sealed class ForcesCommandDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public uint Version { get; init; }
    public int MilitaryUnitCount { get; init; }

    public static Expression<Func<ForcesCommandDef, ForcesCommandDefResponse>> Projection => force =>
        new ForcesCommandDefResponse
        {
            Id = force.Id, Code = force.Code, Name = force.Name,
            ValidFrom = force.ValidFrom, ValidTo = force.ValidTo,
            IsActive = force.IsActive, CreatedDate = force.CreatedDate, Version = force.Version,
            MilitaryUnitCount = force.MilitaryUnits.Count(x => !x.IsDeleted)
        };
}
