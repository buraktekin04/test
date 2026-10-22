using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PlaneTypes.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.PlaneTypes.Commands.Common;
/// <summary>Uçuşa Uçak Tipi Tanımları: benzersizlik, concurrency ve FK doğrulama kuralları.</summary>
internal static class PlaneTypeDefRules
{
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,string code,Guid? ignored,CancellationToken ct)
 {
    if(await db.PlaneTypeDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code.ToUpper()==code.Trim().ToUpperInvariant()&&
       (!ignored.HasValue||x.Id!=ignored.Value),ct))
       throw new PlaneTypeDefConflictException("Tanım kodu zaten kullanılıyor.");
 }
 public static async Task<PlaneTypeDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.PlaneTypeDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new PlaneTypeDefNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 
}
