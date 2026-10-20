using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamReportTypes.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.Common;
/// <summary>Muayene Sebebi Rapor Türleri için eşleştirme tekilliği, üst kayıt durumu ve xmin iş kuralları.</summary>
internal static class ReasonForExamReportTypeRules
{
 public static async Task<ReasonForExamReportType> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.ReasonForExamReportTypes.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
        ?? throw new ReasonForExamReportTypeNotFoundException();
    if(item.Version!=version) throw new DbUpdateConcurrencyException("Eşleştirme başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid reasonId,Guid refId,
        Guid? exclude,CancellationToken ct)
 {
    if(await db.ReasonForExamReportTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted
        &&x.ReasonForExaminationDefId==reasonId&&x.ReportTypeDefId==refId
        
        &&(!exclude.HasValue||x.Id!=exclude.Value),ct))
        throw new ReasonForExamReportTypeConflictException("Aynı muayene sebebi ve koşulları için eşleştirme zaten bulunuyor.");
 }
 /// <summary>Yeni seçilen FK'ler aktif olmalı; değiştirilmemiş pasif geçmiş seçimleri korunur.</summary>
 public static async Task EnsureReferencesAsync(IKLMNDbContext db,Guid reasonId,Guid refId,
    CancellationToken ct,Guid? oldReason=null,Guid? oldRef=null)
 {
    if(reasonId!=oldReason&&!await db.ReasonForExaminationDefs.AnyAsync(x=>x.Id==reasonId&&x.IsActive&&!x.IsDeleted,ct))
        throw new ReasonForExamReportTypeConflictException("Aktif bir muayene sebebi seçiniz.");
    if(refId!=oldRef&&!await db.ReportTypeDefs.AnyAsync(x=>x.Id==refId&&x.IsActive&&!x.IsDeleted,ct))
        throw new ReasonForExamReportTypeConflictException("Aktif bir rapor türü seçiniz.");
 }
}
