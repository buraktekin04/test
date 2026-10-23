using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Defs.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Defs.Commands.Common;
/// <summary>ICD10 kod benzersizliği ve ilişkili istatistikleri koruyan kurallar.</summary>
internal static class ICD10DefRules
{
 public static async Task<ICD10Def> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
  var item=await db.ICD10Defs.IgnoreQueryFilters()
   .SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)??throw new ICD10DefNotFoundException();
  if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından güncellendi.");
  return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,string code,Guid? ignore,CancellationToken ct)
 {
  if(await db.ICD10Defs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&
     x.Code.ToUpper()==code.ToUpper()&&(!ignore.HasValue||x.Id!=ignore.Value),ct))
   throw new ICD10DefConflictException("ICD10 kodu zaten kullanılıyor.");
 }
 public static async Task EnsureUnusedAsync(IKLMNDbContext db,Guid id,bool onlyActive,CancellationToken ct)
 {
  var q=db.ICD10Diagnostics.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.ICD10DefId==id);
  if(onlyActive)q=q.Where(x=>x.IsActive);
  if(await q.AnyAsync(ct))
   throw new ICD10DefConflictException("Bu ICD10 koduna bağlı istatistik kayıtları bulunduğu için işlem yapılamıyor.");
 }
}
