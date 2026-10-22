using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.FlyingMissions.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.FlyingMissions.Commands.Common;
/// <summary>Uçucu Görev Tanımları: benzersizlik, concurrency ve FK doğrulama kuralları.</summary>
internal static class FlyingMissionDefRules
{
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,string code,Guid? ignored,CancellationToken ct)
 {
    if(await db.FlyingMissionDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code.ToUpper()==code.Trim().ToUpperInvariant()&&
       (!ignored.HasValue||x.Id!=ignored.Value),ct))
       throw new FlyingMissionDefConflictException("Tanım kodu zaten kullanılıyor.");
 }
 public static async Task<FlyingMissionDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.FlyingMissionDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new FlyingMissionDefNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 
}
