using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Users.Commands.UpdateUserPermissionSettings;
/// <summary>Kişisel izin/ret kalıcılığı, yetki devri sınırı ve aktif oturumların iptali.</summary>
public sealed class UpdateUserPermissionSettingsCommandHandler(
 IKLMNDbContext db,ICurrentUserService currentUser,IPermissionChecker checker,TimeProvider clock)
 :IRequestHandler<UpdateUserPermissionSettingsCommand,UserMutationResponse>
{
 public async Task<UserMutationResponse> Handle(UpdateUserPermissionSettingsCommand r,CancellationToken ct)
 {
   var user=await db.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==r.UserId&&!x.IsDeleted,ct)
       ??throw new UserNotFoundException();
   if(user.Version!=r.Version)throw new DbUpdateConcurrencyException("Kullanıcı izinleri başka bir işlemde değişti. Listeyi yenileyiniz.");
   if(currentUser.UserId==user.Id)throw new UserConflictException("Kendi özel yetkilerinizi değiştiremezsiniz.");
   if(await db.UserRoles.AnyAsync(x=>x.UserId==user.Id&&x.Role.Code==SystemRoles.Admin,ct))
       throw new UserConflictException("ADMIN hesabı için kişisel yetki değişikliği yapılamaz.");
   var choices=r.Choices.ToDictionary(x=>x.PermissionId,x=>x.IsGranted);
   var wantedIds=choices.Keys.ToArray();
   var available=await db.Permissions.AsNoTracking().Where(x=>wantedIds.Contains(x.Id))
       .Select(x=>new{x.Id,x.Code}).ToListAsync(ct);
   if(available.Count!=choices.Count)throw new UserConflictException("Seçilen izinlerden biri kaldırılmış veya pasif. Paneli yenileyiniz.");
   var active=await db.UserPermissions.IgnoreQueryFilters()
       .Where(x=>x.UserId==user.Id&&!x.IsDeleted).ToListAsync(ct);
   var existing=active.ToDictionary(x=>x.PermissionId);
   // Yalnızca yeni kazandırılan izinlerde delegasyon sınırını uygula.
   // Değiştirilmemiş eski bir kişisel izin, başka alanın güncellenmesini engellemez.
   var actorAdmin=await db.UserRoles.AnyAsync(x=>x.UserId==currentUser.UserId&&x.Role.Code==SystemRoles.Admin,ct);
   if(!actorAdmin)
   {
     foreach(var perm in available.Where(x=>choices[x.Id]==true &&
       (!existing.TryGetValue(x.Id,out var old)||old.IsGranted!=true||!old.IsActive)))
     {
       if(!await checker.HasPermissionAsync(perm.Code,ct))
         throw new UserConflictException("Sahip olmadığınız bir izni başka kullanıcıya veremezsiniz.");
     }
   }
   var changed=false;
   foreach(var choice in r.Choices)
   {
     if(existing.TryGetValue(choice.PermissionId,out var record))
     {
       if(choice.IsGranted is null)
       {
         db.UserPermissions.Remove(record);changed=true;
       }
       else if(record.IsGranted!=choice.IsGranted.Value || !record.IsActive)
       {
         record.IsGranted=choice.IsGranted.Value;record.IsActive=true;changed=true;
       }
     }
     else if(choice.IsGranted.HasValue)
     {
       await db.UserPermissions.AddAsync(new UserPermission{UserId=user.Id,
          PermissionId=choice.PermissionId,IsGranted=choice.IsGranted.Value},ct);
       changed=true;
     }
   }
   if(changed)
   {
     var now=clock.GetUtcNow().UtcDateTime;
     // JWT security stamp ve tüm refresh tokenları iptal edilir; yeni izinler yeni oturumda geçerlidir.
     user.SecurityStamp=Guid.NewGuid().ToString("N");
     var sessions=await db.RefreshTokens.Where(x=>x.UserId==user.Id&&x.RevokedAt==null&&x.ExpiresAt>now)
          .ToListAsync(ct);
     foreach(var session in sessions)
     {
        session.RevokedAt=now;session.RevocationReason="Kişisel yetki güncellendi";
        session.RevokedByIp=currentUser.IpAddress;
     }
     await db.SaveChangesAsync(ct);
   }
   return new UserMutationResponse{Id=user.Id,Version=user.Version};
 }
}
