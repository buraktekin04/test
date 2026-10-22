using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYClassifications.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYClassifications.Commands.Common;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: tekrar, FK, yaşam durumu ve optimistic concurrency güvenliği.</summary>
internal static class SYYClassificationDefRules
{
 public static async Task<SYYClassificationDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
  var item=await db.SYYClassificationDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
    ??throw new SYYClassificationDefNotFoundException();
  if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından güncellendi.");
  return item;
 }
 public static async Task EnsureAsync(IKLMNDbContext db,SYYClassificationDefFields r,Guid? exclude,CancellationToken ct,SYYClassificationDef? old=null)
 {
  if(!KLMN.Domain.Common.ValidityPeriod.HasValidBounds(r.ValidFrom,r.ValidTo))
 throw new SYYClassificationDefConflictException("Geçerlilik tarihi aralığı uygun değil.");
  if(r.SYYDiagnosisDefId!=old?.SYYDiagnosisDefId && !await db.SYYDiagnosisDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.SYYDiagnosisDefId&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYClassificationDefConflictException("SYY tanısı seçimi aktif ve geçerli olmalıdır.");
if(r.ForcesCommandDef!=old?.ForcesCommandDef && !await db.ForcesCommandDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.ForcesCommandDef&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYClassificationDefConflictException("Kuvvet seçimi aktif ve geçerli olmalıdır.");
if(r.RankGroup!=old?.RankGroup && !await db.RankGroupDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.RankGroup&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYClassificationDefConflictException("Rütbe grubu seçimi aktif ve geçerli olmalıdır.");
if(r.MilitaryClassGroup!=old?.MilitaryClassGroup && !await db.MilitaryClassGroupDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.MilitaryClassGroup&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYClassificationDefConflictException("Sınıf grubu seçimi aktif ve geçerli olmalıdır.");
if(r.HcDecision!=old?.HcDecision && !await db.HcConnectedDecisionDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.HcDecision&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYClassificationDefConflictException("Karar seçimi aktif ve geçerli olmalıdır.");
  if(!await db.MilitaryClassGroupDefs.IgnoreQueryFilters()
  .AnyAsync(x=>x.Id==r.MilitaryClassGroup&&!x.IsDeleted&&x.ForcesCommandId==r.ForcesCommandDef,ct))
 throw new SYYClassificationDefConflictException("Seçilen sınıf grubu, seçilen kuvvet komutanlığına bağlı olmalıdır.");
  if(await db.SYYClassificationDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==r.SYYDiagnosisDefId&&x.ForcesCommandDef==r.ForcesCommandDef&&x.RankGroup==r.RankGroup&&x.MilitaryClassGroup==r.MilitaryClassGroup&&x.HcDecision==r.HcDecision&&x.ValidFrom==r.ValidFrom&&
   (!exclude.HasValue||x.Id!=exclude.Value),ct))
   throw new SYYClassificationDefConflictException("Aynı tanım ve koşullar için kayıt bulunuyor.");
 }
}
/// <summary>Command kontrolünde kullanılan ortak alan sözleşmesi.</summary>
internal sealed record SYYClassificationDefFields(Guid SYYDiagnosisDefId,Guid ForcesCommandDef,Guid RankGroup,Guid MilitaryClassGroup,Guid HcDecision,DateTime? ValidFrom,DateTime? ValidTo);
