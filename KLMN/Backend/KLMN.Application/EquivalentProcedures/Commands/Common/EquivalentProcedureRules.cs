using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.EquivalentProcedures.Commands.Common;
/// <summary>İki yönlü eşdeğerliği kanonik sırayla korur.</summary>
internal static class EquivalentProcedureRules
{
 public static (Guid Left,Guid Right) Normalize(Guid first,Guid second)
 {
   if(first==second)throw new EquivalentProcedureConflictException("Tetkik kendisiyle eşdeğer olamaz.");
   return first.CompareTo(second)<0?(first,second):(second,first);
 }
 public static async Task EnsureReferencesAsync(IKLMNDbContext db,Guid first,Guid second,CancellationToken ct,
  EquivalentProcedureDef? original=null)
 {
   foreach(var id in new[]{first,second}.Distinct())
   {
     if(id==original?.ProcedureDefId||id==original?.EquivalentProcedureId)continue;
     if(!await db.ProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==id&&!x.IsDeleted&&x.IsActive,ct))
       throw new EquivalentProcedureConflictException("Eşleştirmek için aktif bir tetkik seçiniz.");
   }
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid first,Guid second,Guid? except,CancellationToken ct)
 {
   var (left,right)=Normalize(first,second);
   // Tarihsel ters yönlü kayıtlar da reddedilir.
   if(await db.EquivalentProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted &&
      (!except.HasValue||x.Id!=except.Value) &&
      ((x.ProcedureDefId==left&&x.EquivalentProcedureId==right) ||
       (x.ProcedureDefId==right&&x.EquivalentProcedureId==left)),ct))
       throw new EquivalentProcedureConflictException("Bu iki tetkik zaten eşdeğer olarak ilişkilendirilmiş.");
 }
 public static async Task<EquivalentProcedureDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
   var item=await db.EquivalentProcedureDefs.IgnoreQueryFilters()
     .SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
       ??throw new EquivalentProcedureNotFoundException();
   if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
   return item;
 }
}
