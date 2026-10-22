using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisBranches.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.Common;
/// <summary>SYY Gereken Branşlar: tekrar, FK, yaşam durumu ve optimistic concurrency güvenliği.</summary>
internal static class SYYDiagnosisBranchDefRules
{
 public static async Task<SYYDiagnosisBranchDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
  var item=await db.SYYDiagnosisBranchDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
    ??throw new SYYDiagnosisBranchDefNotFoundException();
  if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından güncellendi.");
  return item;
 }
 public static async Task EnsureAsync(IKLMNDbContext db,SYYDiagnosisBranchDefFields r,Guid? exclude,CancellationToken ct,SYYDiagnosisBranchDef? old=null)
 {
  if(r.StartingAge>r.EndingAge)throw new SYYDiagnosisBranchDefConflictException("Başlangıç yaşı bitiş yaşından büyük olamaz.");
  if(r.SYYDiagnosisDefId!=old?.SYYDiagnosisDefId && !await db.SYYDiagnosisDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.SYYDiagnosisDefId&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYDiagnosisBranchDefConflictException("SYY tanısı seçimi aktif ve geçerli olmalıdır.");
if(r.BranchDefId!=old?.BranchDefId && !await db.BranchDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.BranchDefId&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYDiagnosisBranchDefConflictException("Kuvvet seçimi aktif ve geçerli olmalıdır.");
  
  if(await db.SYYDiagnosisBranchDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==r.SYYDiagnosisDefId&&x.BranchDefId==r.BranchDefId&&x.StartingAge==r.StartingAge&&x.EndingAge==r.EndingAge&&x.Gender==r.Gender&&
   (!exclude.HasValue||x.Id!=exclude.Value),ct))
   throw new SYYDiagnosisBranchDefConflictException("Aynı tanım ve koşullar için kayıt bulunuyor.");
 }
}
/// <summary>Command kontrolünde kullanılan ortak alan sözleşmesi.</summary>
internal sealed record SYYDiagnosisBranchDefFields(Guid SYYDiagnosisDefId,Guid BranchDefId,int StartingAge,int EndingAge,int Gender);
