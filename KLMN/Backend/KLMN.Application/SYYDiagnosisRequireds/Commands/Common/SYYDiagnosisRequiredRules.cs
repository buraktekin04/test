using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisRequireds.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.Common;
/// <summary>Tanı başına birden fazla koşullu kural; madde listesi ayrı tabloda ve tek işlemde güncellenir.</summary>
internal static class SYYDiagnosisRequiredRules
{
 public static async Task<SYYDiagnosisRequired> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
 {
  var item=await db.SYYDiagnosisRequireds.IgnoreQueryFilters()
    .Include(x=>x.Details).SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
    ??throw new SYYDiagnosisRequiredNotFoundException();
  if(item.Version!=version)throw new DbUpdateConcurrencyException("Kural başka kullanıcı tarafından değiştirildi.");
  return item;
 }
 public static async Task ValidateDiagnosisAsync(IKLMNDbContext db,Guid id,Guid? excludeRuleId,CancellationToken ct)
 {
  var diagnosis=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>x.Id==id&&!x.IsDeleted).Select(x=>new{x.IsActive}).SingleOrDefaultAsync(ct);
  var unchanged=excludeRuleId.HasValue&&await db.SYYDiagnosisRequireds.IgnoreQueryFilters()
   .AnyAsync(x=>x.Id==excludeRuleId.Value&&x.SYYDiagnosisDefId==id&&!x.IsDeleted,ct);
  if(diagnosis is null||(!diagnosis.IsActive&&!unchanged))
    throw new SYYDiagnosisRequiredConflictException("Aktif bir SYY tanısı seçiniz.");
  // Bir tanıda birbirinden farklı birden fazla onay kuralı bulunabilir.
 }
}
