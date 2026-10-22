using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AnnotationReasons.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.AnnotationReasons.Commands.Common;
/// <summary>Sevk Sebebi Tanımları: benzersizlik, concurrency ve FK doğrulama kuralları.</summary>
internal static class AnnotationReasonDefRules
{
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,string code,Guid? ignored,CancellationToken ct)
 {
    if(await db.AnnotationReasonDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code.ToUpper()==code.Trim().ToUpperInvariant()&&
       (!ignored.HasValue||x.Id!=ignored.Value),ct))
       throw new AnnotationReasonDefConflictException("Tanım kodu zaten kullanılıyor.");
 }
 public static async Task<AnnotationReasonDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.AnnotationReasonDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new AnnotationReasonDefNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 
}
