using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.Common;
/// <summary>İlişki benzersizliği, yalnız aktif referanslara yeni atama ve optimistic concurrency.</summary>
internal static class SYYDiagnosisDecisionRules
{
 public static async Task<SYYDiagnosisDecision> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.SYYDiagnosisDecisions.IgnoreQueryFilters()
      .SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
      ??throw new SYYDiagnosisDecisionNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("İstatistik kaydı başka bir kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid syy,Guid decision,Guid admission,Guid? military,Guid? exclude,CancellationToken ct)
 {
    if(await db.SYYDiagnosisDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&
      x.SYYDiagnosisDefId==syy&&x.HcDecisionDefId==decision&&
      x.HcAdmissionTypeDefId==admission&&x.MilitaryClassDefId==military&&
      (!exclude.HasValue||x.Id!=exclude.Value),ct))
       throw new SYYDiagnosisDecisionConflictException("Bu tanı, karar, başvuru grubu ve askerî sınıf eşleştirmesi zaten kayıtlı.");
 }
 public static async Task ValidateReferencesAsync(IKLMNDbContext db,Guid syy,Guid decision,Guid admission,Guid? military,
   CancellationToken ct,SYYDiagnosisDecision? old=null)
 {
   if(syy!=old?.SYYDiagnosisDefId&&!await db.SYYDiagnosisDefs.AnyAsync(x=>x.Id==syy&&x.IsActive&&!x.IsDeleted,ct))
      throw new SYYDiagnosisDecisionConflictException("Aktif bir SYY tanısı seçiniz.");
   if(decision!=old?.HcDecisionDefId&&!await db.HcDecisionDefs.AnyAsync(x=>x.Id==decision&&x.IsActive&&!x.IsDeleted,ct))
      throw new SYYDiagnosisDecisionConflictException("Aktif bir kurul kararı seçiniz.");
   if(admission!=old?.HcAdmissionTypeDefId&&!await db.HcAdmissionTypeDefs.AnyAsync(x=>x.Id==admission&&x.IsActive&&!x.IsDeleted,ct))
      throw new SYYDiagnosisDecisionConflictException("Aktif bir başvuru grubu seçiniz.");
   if(military.HasValue&&military!=old?.MilitaryClassDefId&&
      !await db.MilitaryClassDefs.AnyAsync(x=>x.Id==military.Value&&x.IsActive&&!x.IsDeleted,ct))
      throw new SYYDiagnosisDecisionConflictException("Aktif bir askerî sınıf seçiniz.");
 }
 public static async Task ValidateActiveReferencesAsync(IKLMNDbContext db,SYYDiagnosisDecision x,CancellationToken ct)
 {
    if(!await db.SYYDiagnosisDefs.AnyAsync(r=>r.Id==x.SYYDiagnosisDefId&&r.IsActive,ct)||
       !await db.HcDecisionDefs.AnyAsync(r=>r.Id==x.HcDecisionDefId&&r.IsActive,ct)||
       !await db.HcAdmissionTypeDefs.AnyAsync(r=>r.Id==x.HcAdmissionTypeDefId&&r.IsActive,ct)||
       (x.MilitaryClassDefId.HasValue&&
         !await db.MilitaryClassDefs.AnyAsync(r=>r.Id==x.MilitaryClassDefId.Value&&r.IsActive,ct)))
       throw new SYYDiagnosisDecisionConflictException("Pasif bir referansı bulunan istatistik aktifleştirilemez.");
 }
}
