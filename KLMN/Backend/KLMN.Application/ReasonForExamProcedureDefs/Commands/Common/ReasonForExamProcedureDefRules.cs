using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamProcedureDefs.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.Common;
/// <summary>Gereken Tetkikler için eşleştirme tekilliği, üst kayıt durumu ve xmin iş kuralları.</summary>
internal static class ReasonForExamProcedureDefRules
{
 public static async Task<ReasonForExamProcedureDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.ReasonForExamProcedureDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
        ?? throw new ReasonForExamProcedureDefNotFoundException();
    if(item.Version!=version) throw new DbUpdateConcurrencyException("Eşleştirme başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid reasonId,Guid refId,
        int startingAge,int endingAge,int gender,Guid? exclude,CancellationToken ct)
 {
    if(await db.ReasonForExamProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted
        &&x.ReasonForExaminationDefId==reasonId&&x.ProcedureDefId==refId
        &&x.StartingAge==startingAge&&x.EndingAge==endingAge&&x.Gender==gender
        &&(!exclude.HasValue||x.Id!=exclude.Value),ct))
        throw new ReasonForExamProcedureDefConflictException("Aynı muayene sebebi ve koşulları için eşleştirme zaten bulunuyor.");
 }
 /// <summary>Yeni seçilen FK'ler aktif olmalı; değiştirilmemiş pasif geçmiş seçimleri korunur.</summary>
 public static async Task EnsureReferencesAsync(IKLMNDbContext db,Guid reasonId,Guid refId,
    CancellationToken ct,Guid? oldReason=null,Guid? oldRef=null)
 {
    if(reasonId!=oldReason&&!await db.ReasonForExaminationDefs.AnyAsync(x=>x.Id==reasonId&&x.IsActive&&!x.IsDeleted,ct))
        throw new ReasonForExamProcedureDefConflictException("Aktif bir muayene sebebi seçiniz.");
    if(refId!=oldRef&&!await db.ProcedureDefs.AnyAsync(x=>x.Id==refId&&x.IsActive&&!x.IsDeleted,ct))
        throw new ReasonForExamProcedureDefConflictException("Aktif bir tetkik seçiniz.");
 }
}
