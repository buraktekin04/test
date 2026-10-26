using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Users.Queries.GetUserPermissionSettings;
/// <summary>Rolden devralınan ve özel olarak izin verilen/reddedilen hakları yan yana sunar.</summary>
public sealed class GetUserPermissionSettingsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetUserPermissionSettingsQuery,UserPermissionSettingsResponse>
{
 public async Task<UserPermissionSettingsResponse> Handle(GetUserPermissionSettingsQuery r,CancellationToken ct)
 {
   var user=await db.Users.IgnoreQueryFilters().AsNoTracking()
     .Where(x=>x.Id==r.UserId&&!x.IsDeleted).Select(x=>new{x.Id,x.Version}).SingleOrDefaultAsync(ct)
       ??throw new UserNotFoundException();
   var roles=await db.UserRoles.AsNoTracking().Where(x=>x.UserId==r.UserId)
       .Select(x=>new{x.RoleId,x.Role.Code}).ToListAsync(ct);
   var isAdmin=roles.Any(x=>x.Code==SystemRoles.Admin);
   var roleIds=roles.Select(x=>x.RoleId).ToArray();
   var inherited=(await db.RolePermissions.AsNoTracking().Where(x=>roleIds.Contains(x.RoleId))
       .Select(x=>x.PermissionId).Distinct().ToListAsync(ct)).ToHashSet();
   var overrides=(await db.UserPermissions.AsNoTracking().Where(x=>x.UserId==r.UserId)
       .Select(x=>new{x.PermissionId,x.IsGranted}).ToListAsync(ct))
       .ToDictionary(x=>x.PermissionId,x=>(bool?)x.IsGranted);
   var permissions=await db.Permissions.AsNoTracking().OrderBy(x=>x.Module).ThenBy(x=>x.SortOrder)
       .ThenBy(x=>x.Name).Select(x=>new{x.Id,x.Code,x.Name,x.Module,x.SortOrder}).ToListAsync(ct);
   var rows=permissions.Select(p=>{
       var grantedByRole=inherited.Contains(p.Id);
       var custom=overrides.TryGetValue(p.Id,out var value)?value:null;
       return new UserPermissionSettingResponse(p.Id,p.Code,p.Name,p.Module,p.SortOrder,
           grantedByRole,custom,isAdmin||(custom??grantedByRole));
   }).ToList();
   return new UserPermissionSettingsResponse(user.Id,user.Version,isAdmin,rows);
 }
}
