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
    // Yeni bağımlı tablolar varsa anlaşılır 409, PostgreSQL FK hatası değil.
    var classification=db.SYYClassificationDefs.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==id);
    var branches=db.SYYDiagnosisBranchDefs.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==id);
    var procedures=db.SYYDiagnosisProcedureDefs.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==id);
    var required=db.SYYDiagnosisRequireds.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==id);
    if(onlyActive){
      classification=classification.Where(x=>x.IsActive);branches=branches.Where(x=>x.IsActive);
      procedures=procedures.Where(x=>x.IsActive);required=required.Where(x=>x.IsActive);
    }
    if(await classification.AnyAsync(ct)||await branches.AnyAsync(ct)||
       await procedures.AnyAsync(ct)||await required.AnyAsync(ct))
      throw new SYYDiagnosisDefConflictException("SYY tanısına bağlı sınıflandırma, kural, branş veya tetkik kaydı bulunuyor.");
    var decisions=db.SYYDiagnosisDecisions.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==id);
    if(onlyActive)decisions=decisions.Where(x=>x.IsActive);
    if(await decisions.AnyAsync(ct))
       throw new SYYDiagnosisDefConflictException("Tanıya bağlı SYY karar istatistikleri bulunduğundan işlem yapılamaz.");
    var icd=db.ICD10Diagnostics.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==id);
    if(onlyActive)icd=icd.Where(x=>x.IsActive);
    if(await icd.AnyAsync(ct))
      throw new SYYDiagnosisDefConflictException("Bu tanıya bağlı ICD10 SYY istatistik kayıtları bulunduğundan işlem yapılamıyor.");

 }

}
