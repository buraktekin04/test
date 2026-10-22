using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnoses.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnoses.Commands.Common;
/// <summary>SYY Tanı Listesi (M/D/F): benzersizlik, concurrency ve FK doğrulama kuralları.</summary>
internal static class SYYDiagnosisDefRules
{
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,string code,Guid? ignored,CancellationToken ct)
 {
    if(await db.SYYDiagnosisDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code.ToUpper()==code.Trim().ToUpperInvariant()&&
       (!ignored.HasValue||x.Id!=ignored.Value),ct))
       throw new SYYDiagnosisDefConflictException("Tanım kodu zaten kullanılıyor.");
 }
 public static async Task<SYYDiagnosisDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.SYYDiagnosisDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new SYYDiagnosisDefNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureRelationsAsync(IKLMNDbContext db,Guid id,int type,Guid? parent,int? slice,
 Guid? previousParent,int? previousType,int? previousSlice,CancellationToken ct)
 {
   if(type!=previousType&&!await db.SYYDiagnosisTypeDefs.IgnoreQueryFilters()
      .AnyAsync(x=>x.Code==type&&!x.IsDeleted&&x.IsActive,ct))
        throw new SYYDiagnosisDefConflictException("Aktif bir Tanı Türü seçiniz.");
   if(slice.HasValue&&slice!=previousSlice&&!await db.SYYDiagnosisSliceDefs.IgnoreQueryFilters()
      .AnyAsync(x=>x.Code==slice.Value&&!x.IsDeleted&&x.IsActive,ct))
        throw new SYYDiagnosisDefConflictException("Aktif bir Dilim seçiniz.");
   if(parent.HasValue&&parent!=previousParent)
   {
     var visited=new HashSet<Guid>{id};
     Guid? at=parent;
     while(at.HasValue)
     {
        if(!visited.Add(at.Value))
          throw new SYYDiagnosisDefConflictException("Bağlı kayıt ilişkisinde döngü oluşturulamaz.");
        var current=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
          .Where(x=>x.Id==at.Value&&!x.IsDeleted)
          .Select(x=>new {x.AttachedRecNo,x.IsActive}).SingleOrDefaultAsync(ct);
        if(current is null||!current.IsActive)
          throw new SYYDiagnosisDefConflictException("Bağlı tanı aktif olmalıdır.");
        at=current.AttachedRecNo;
     }
   }
 }
 /// <summary>Bağlı alt tanıların silinme/pasifleştirme güvenliği.</summary>
 public static async Task EnsureNoChildrenAsync(IKLMNDbContext db,Guid id,bool onlyActive,CancellationToken ct)
 {
    var q=db.SYYDiagnosisDefs.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.AttachedRecNo==id);
    if(onlyActive)q=q.Where(x=>x.IsActive);
    if(await q.AnyAsync(ct))throw new SYYDiagnosisDefConflictException("Tanıya bağlı alt tanılar bulunuyor.");
 }

}
