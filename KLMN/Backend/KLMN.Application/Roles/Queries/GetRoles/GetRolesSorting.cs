using System.Linq.Expressions;
using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Application.Roles.Queries.GetRoles;

/// <summary>Liste ve Excel sorgularının izin verilen sıralama kolonları.</summary>
public static class GetRolesSorting
{
    public static readonly DefinitionSort<Role> Definition = new(
        new Dictionary<string, Expression<Func<Role, object?>>>
        {
            ["name"] = x => x.Name,
            ["code"] = x => x.Code,
            ["description"] = x => x.Description,
            ["isSystemRole"] = x => x.IsSystemRole,
            ["isActive"] = x => x.IsActive,
            ["userCount"] = x => x.UserRoles.Count(ur => ur.IsActive && !ur.IsDeleted && ur.User.IsActive && !ur.User.IsDeleted),
            ["permissionCount"] = x => x.RolePermissions.Count(rp => rp.IsActive && !rp.IsDeleted && rp.Permission.IsActive && !rp.Permission.IsDeleted),
            ["createdDate"] = x => x.CreatedDate,
        });
}
