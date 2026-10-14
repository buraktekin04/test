using System.Linq.Expressions;
using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>Liste ve Excel sorgularının izin verilen sıralama kolonları.</summary>
public static class GetUsersSorting
{
    public static readonly DefinitionSort<User> Definition = new(
        new Dictionary<string, Expression<Func<User, object?>>>
        {
            ["fullName"] = x => (x.FirstName + " " + x.LastName).Trim(),
            ["userName"] = x => x.UserName,
            ["email"] = x => x.Email,
            ["militaryUnit"] = x => x.MilitaryUnitDef != null ? x.MilitaryUnitDef.Name : null,
            ["roles"] = x => x.UserRoles.Where(ur => ur.IsActive && !ur.IsDeleted && ur.Role.IsActive && !ur.Role.IsDeleted).Select(ur => ur.Role.Code).OrderBy(code => code).FirstOrDefault(),
            ["isActive"] = x => x.IsActive,
            ["clockLimit"] = x => !x.ClockLimit,
            ["validFrom"] = x => x.ValidFrom,
            ["validTo"] = x => x.ValidTo,
            ["createdDate"] = x => x.CreatedDate,
        });
}
