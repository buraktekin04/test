using System.Linq.Expressions;
using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefs;

/// <summary>Liste ve Excel sorgularının izin verilen sıralama kolonları.</summary>
public static class GetForcesCommandDefsSorting
{
    public static readonly DefinitionSort<ForcesCommandDef> Definition = new(
        new Dictionary<string, Expression<Func<ForcesCommandDef, object?>>>
        {
            ["name"] = x => x.Name,
            ["code"] = x => x.Code,
            ["isActive"] = x => x.IsActive,
            ["militaryUnitCount"] = x => x.MilitaryUnits.Count(u => !u.IsDeleted),
            ["validFrom"] = x => x.ValidFrom,
            ["validTo"] = x => x.ValidTo,
            ["createdDate"] = x => x.CreatedDate,
        });
}
