using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamBranchDefs.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.Common;
/// <summary>Muayene Gereken Branşlar için eşleştirme tekilliği, üst kayıt durumu ve xmin iş kuralları.</summary>
internal static class ReasonForExamBranchDefRules
{
 public static async Task<ReasonForExamBranchDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.ReasonForExamBranchDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
        ?? throw new ReasonForExamBranchDefNotFoundException();
    if(item.Version!=version) throw new DbUpdateConcurrencyException("Eşleştirme başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid reasonId,Guid refId,
        int startingAge,int endingAge,int gender,Guid? exclude,CancellationToken ct)
 {
    if(await db.ReasonForExamBranchDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted
        &&x.ReasonForExaminationDefId==reasonId&&x.BranchDefId==refId
        &&x.StartingAge==startingAge&&x.EndingAge==endingAge&&x.Gender==gender
        &&(!exclude.HasValue||x.Id!=exclude.Value),ct))
        throw new ReasonForExamBranchDefConflictException("Aynı muayene sebebi ve koşulları için eşleştirme zaten bulunuyor.");
 }
 /// <summary>Yeni seçilen FK'ler aktif olmalı; değiştirilmemiş pasif geçmiş seçimleri korunur.</summary>
 public static async Task EnsureReferencesAsync(IKLMNDbContext db,Guid reasonId,Guid refId,
    CancellationToken ct,Guid? oldReason=null,Guid? oldRef=null)
 {
    if(reasonId!=oldReason&&!await db.ReasonForExaminationDefs.AnyAsync(x=>x.Id==reasonId&&x.IsActive&&!x.IsDeleted,ct))
        throw new ReasonForExamBranchDefConflictException("Aktif bir muayene sebebi seçiniz.");
    if(refId!=oldRef&&!await db.BranchDefs.AnyAsync(x=>x.Id==refId&&x.IsActive&&!x.IsDeleted,ct))
        throw new ReasonForExamBranchDefConflictException("Aktif bir branş seçiniz.");
 }
}
