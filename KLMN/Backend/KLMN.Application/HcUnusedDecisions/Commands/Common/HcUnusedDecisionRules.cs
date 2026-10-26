using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedDecisions.Commands.Common;
/// <summary>Birlikte Verilmeyecek Kararlar: referans durumu, çift tekilliği ve xmin denetimi.</summary>
internal static class HcUnusedDecisionRules
{
 public static async Task<HcUnusedDecision> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
    var item=await db.HcUnusedDecisions.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
        ?? throw new HcUnusedDecisionNotFoundException();
    if(item.Version!=version)throw new DbUpdateConcurrencyException("Eşleştirme başka kullanıcı tarafından değiştirildi.");
    return item;
 }
 public static async Task EnsureUniqueAsync(IKLMNDbContext db,Guid decisionId,Guid refId,Guid? exclude,CancellationToken ct)
 {
    if(await db.HcUnusedDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcDecisionDefId==decisionId&&x.UnusedHcDecisionDefId==refId&&(!exclude.HasValue||x.Id!=exclude.Value),ct))
       throw new HcUnusedDecisionConflictException("Bu kurul kararı için aynı eşleştirme zaten bulunuyor.");
 }
 /// <summary>Yeni FK aktif ve silinmemiş olmalı; düzenlemede değiştirilmemiş tarihsel pasif kayıt korunur.</summary>
 public static async Task EnsureReferencesAsync(IKLMNDbContext db,Guid decisionId,Guid refId,CancellationToken ct,Guid? oldDecision=null,Guid? oldRef=null)
 {
    if(decisionId!=oldDecision&&!await db.HcDecisionDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==decisionId&&!x.IsDeleted&&x.IsActive,ct))
      throw new HcUnusedDecisionConflictException("Aktif bir kurul kararı seçiniz.");
    if(refId!=oldRef&&!await db.HcDecisionDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==refId&&!x.IsDeleted&&x.IsActive,ct))
      throw new HcUnusedDecisionConflictException("Aktif bir referans kaydı seçiniz.");
    if(decisionId==refId)throw new HcUnusedDecisionConflictException("Kurul kararı kendisiyle birlikte verilemez.");
 }
}
