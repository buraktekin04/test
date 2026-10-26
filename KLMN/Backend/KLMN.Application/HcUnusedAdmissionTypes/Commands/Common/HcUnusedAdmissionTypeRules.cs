using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.Common;
/// <summary>Kullanılmayacak Başvuru Grupları: referans durumu, çift tekilliği ve xmin denetimi.</summary>
internal static class HcUnusedAdmissionTypeRules
{
 public static async Task<HcUnusedAdmissionType> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.HcUnusedAdmissionTypes.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
        ?? throw new HcUnusedAdmissionTypeNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Eşleştirme başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid decisionId,Guid refId,Guid? exclude,CancellationToken ct)
 {
    if(await db.HcUnusedAdmissionTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcDecisionDefId==decisionId&&x.HcAdmissionTypeDefId==refId&&(!exclude.HasValue||x.Id!=exclude.Value),ct))
       throw new HcUnusedAdmissionTypeConflictException("Bu kurul kararı için aynı eşleştirme zaten bulunuyor.");
 }
 /// <summary>Yeni FK aktif ve silinmemiş olmalı; düzenlemede değiştirilmemiş tarihsel pasif kayıt korunur.</summary>
 public static async Task EnsureReferencesAsync(IKLMNDbContext db,Guid decisionId,Guid refId,CancellationToken ct,Guid? oldDecision=null,Guid? oldRef=null)
 {
    if(decisionId!=oldDecision&&!await db.HcDecisionDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==decisionId&&!x.IsDeleted&&x.IsActive,ct))
      throw new HcUnusedAdmissionTypeConflictException("Aktif bir kurul kararı seçiniz.");
    if(refId!=oldRef&&!await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==refId&&!x.IsDeleted&&x.IsActive,ct))
      throw new HcUnusedAdmissionTypeConflictException("Aktif bir referans kaydı seçiniz.");
    
 }
}
