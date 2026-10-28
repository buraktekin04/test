using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisSlices.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisSlices.Commands.Common;
/// <summary>SYY Dilim Tanımları: benzersizlik, concurrency ve FK doğrulama kuralları.</summary>
internal static class SYYDiagnosisSliceDefRules
{
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,int code,Guid? ignored,CancellationToken ct)
 {
    if(await db.SYYDiagnosisSliceDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code==code&&(!ignored.HasValue||x.Id!=ignored.Value),ct))
        throw new SYYDiagnosisSliceDefConflictException("Bu kod silinmemiş başka bir tanımda zaten kullanılıyor.");
 }
 public static async Task<SYYDiagnosisSliceDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.SYYDiagnosisSliceDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new SYYDiagnosisSliceDefNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUnusedAsync(IKLMNDbContext db,Guid id,CancellationToken ct)
 {
  if(await db.SYYDiagnosisDefs.IgnoreQueryFilters()
   .AnyAsync(x=>x.SYYDiagnosisSliceDefId==id&&!x.IsDeleted,ct))
    throw new SYYDiagnosisSliceDefConflictException("Bu tanıma bağlı SYY kayıtları bulunduğundan silinemez.");
 }
}
