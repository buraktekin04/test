using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisProcedures.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.Common;
/// <summary>SYY Gereken Tetkikler: tekrar, FK, yaşam durumu ve optimistic concurrency güvenliği.</summary>
internal static class SYYDiagnosisProcedureDefRules
{
 public static async Task<SYYDiagnosisProcedureDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
  var item=await db.SYYDiagnosisProcedureDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
    ??throw new SYYDiagnosisProcedureDefNotFoundException();
  if(item.Version!=version)throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından güncellendi.");
  return item;
 }
 public static async Task EnsureAsync(IKLMNDbContext db,SYYDiagnosisProcedureDefFields r,Guid? exclude,CancellationToken ct,SYYDiagnosisProcedureDef? old=null)
 {
  if(r.StartingAge>r.EndingAge)throw new SYYDiagnosisProcedureDefConflictException("Başlangıç yaşı bitiş yaşından büyük olamaz.");
  if(r.SYYDiagnosisDefId!=old?.SYYDiagnosisDefId && !await db.SYYDiagnosisDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.SYYDiagnosisDefId&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYDiagnosisProcedureDefConflictException("SYY tanısı seçimi aktif ve geçerli olmalıdır.");
if(r.ProcedureDefId!=old?.ProcedureDefId && !await db.ProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>x.Id==r.ProcedureDefId&&!x.IsDeleted&&x.IsActive,ct))
 throw new SYYDiagnosisProcedureDefConflictException("Kuvvet seçimi aktif ve geçerli olmalıdır.");
  
  if(await db.SYYDiagnosisProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.SYYDiagnosisDefId==r.SYYDiagnosisDefId&&x.ProcedureDefId==r.ProcedureDefId&&x.StartingAge==r.StartingAge&&x.EndingAge==r.EndingAge&&x.Gender==r.Gender&&
   (!exclude.HasValue||x.Id!=exclude.Value),ct))
   throw new SYYDiagnosisProcedureDefConflictException("Aynı tanım ve koşullar için kayıt bulunuyor.");
 }
}
/// <summary>Command kontrolünde kullanılan ortak alan sözleşmesi.</summary>
internal sealed record SYYDiagnosisProcedureDefFields(Guid SYYDiagnosisDefId,Guid ProcedureDefId,int StartingAge,int EndingAge,int Gender);
