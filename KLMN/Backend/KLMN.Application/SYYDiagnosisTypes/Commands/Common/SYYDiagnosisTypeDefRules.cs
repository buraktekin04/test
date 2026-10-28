using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisTypes.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisTypes.Commands.Common;
/// <summary>SYY Tanı Türü Tanımları: benzersizlik, concurrency ve FK doğrulama kuralları.</summary>
internal static class SYYDiagnosisTypeDefRules
{
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,int code,Guid? ignored,CancellationToken ct)
 {
    if(await db.SYYDiagnosisTypeDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code==code&&(!ignored.HasValue||x.Id!=ignored.Value),ct))
        throw new SYYDiagnosisTypeDefConflictException("Bu kod silinmemiş başka bir tanımda zaten kullanılıyor.");
 }
 public static async Task<SYYDiagnosisTypeDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.SYYDiagnosisTypeDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new SYYDiagnosisTypeDefNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUnusedAsync(IKLMNDbContext db,Guid id,CancellationToken ct)
 {
  if(await db.SYYDiagnosisDefs.IgnoreQueryFilters()
   .AnyAsync(x=>x.SYYDiagnosisTypeDefId==id&&!x.IsDeleted,ct))
    throw new SYYDiagnosisTypeDefConflictException("Bu tanıma bağlı SYY kayıtları bulunduğundan silinemez.");
 }
}
