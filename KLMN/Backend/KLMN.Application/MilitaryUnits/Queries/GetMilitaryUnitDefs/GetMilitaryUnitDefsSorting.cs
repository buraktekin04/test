using System.Linq.Expressions;
using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs;

/// <summary>Liste ve Excel sorgularının izin verilen sıralama kolonları.</summary>
public static class GetMilitaryUnitDefsSorting
{
    public static readonly DefinitionSort<MilitaryUnitDef> Definition = new(
        new Dictionary<string, Expression<Func<MilitaryUnitDef, object?>>>
        {
            ["name"] = x => x.Name,
            ["code"] = x => x.Code,
            ["externalCode"] = x => x.ExternalCode,
            ["forcesCommandName"] = x => x.ForceCommand != null ? x.ForceCommand.Name : null,
            ["connectedTopUnitName"] = x => x.ConnectedTopUnit != null ? x.ConnectedTopUnit.Name : null,
            ["isActive"] = x => x.IsActive,
            ["userCount"] = x => x.Users.Count(u => !u.IsDeleted),
            ["childCount"] = x => x.Children.Count(u => !u.IsDeleted),
            ["validFrom"] = x => x.ValidFrom,
            ["validTo"] = x => x.ValidTo,
            ["createdDate"] = x => x.CreatedDate,
        });
}
