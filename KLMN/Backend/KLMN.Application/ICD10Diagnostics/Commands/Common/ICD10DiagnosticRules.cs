using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Diagnostics.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Diagnostics.Commands.Common;
/// <summary>İstatistik anahtarı, aktif referanslar, bütünlük ve xmin güvenliği.</summary>
internal static class ICD10DiagnosticRules
{
 public static async Task<ICD10Diagnostic> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
  var item=await db.ICD10Diagnostics.IgnoreQueryFilters()
   .SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)??throw new ICD10DiagnosticNotFoundException();
  if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından güncellendi.");
  return item;
 }
 public static async Task ValidateReferencesAsync(IKLMNDbContext db,Guid icdId,Guid syyId,Guid admissionId,
  CancellationToken ct,ICD10Diagnostic? old=null)
 {
  if(icdId!=old?.ICD10DefId&&!await db.ICD10Defs.AnyAsync(x=>x.Id==icdId&&x.IsActive&&!x.IsDeleted,ct))
   throw new ICD10DiagnosticConflictException("Aktif bir ICD10 kodu seçiniz.");
  if(syyId!=old?.SYYDiagnosisDefId&&!await db.SYYDiagnosisDefs.AnyAsync(x=>x.Id==syyId&&x.IsActive&&!x.IsDeleted,ct))
   throw new ICD10DiagnosticConflictException("Aktif bir SYY tanısı seçiniz.");
  if(admissionId!=old?.HcAdmissionTypeDefId&&!await db.HcAdmissionTypeDefs.AnyAsync(x=>x.Id==admissionId&&x.IsActive&&!x.IsDeleted,ct))
   throw new ICD10DiagnosticConflictException("Aktif bir Başvuru Grubu seçiniz.");
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid icdId,Guid syyId,Guid admissionId,
  Guid? ignore,CancellationToken ct)
 {
  if(await db.ICD10Diagnostics.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&
      x.ICD10DefId==icdId&&x.SYYDiagnosisDefId==syyId&&x.HcAdmissionTypeDefId==admissionId&&
      (!ignore.HasValue||x.Id!=ignore.Value),ct))
   throw new ICD10DiagnosticConflictException("Bu ICD10, SYY Tanısı ve Başvuru Grubu eşleştirmesi zaten tanımlı.");
 }
 public static async Task ValidateActiveReferencesAsync(IKLMNDbContext db,ICD10Diagnostic row,CancellationToken ct)
 {
  if(!await db.ICD10Defs.AnyAsync(x=>x.Id==row.ICD10DefId&&x.IsActive&&!x.IsDeleted,ct)||
     !await db.SYYDiagnosisDefs.AnyAsync(x=>x.Id==row.SYYDiagnosisDefId&&x.IsActive&&!x.IsDeleted,ct)||
     !await db.HcAdmissionTypeDefs.AnyAsync(x=>x.Id==row.HcAdmissionTypeDefId&&x.IsActive&&!x.IsDeleted,ct))
   throw new ICD10DiagnosticConflictException("Pasif referanslara bağlı istatistik kaydı aktifleştirilemez.");
 }
}
