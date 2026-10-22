using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefOptions;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: seçili pasif tarihsel kayıtlar hariç aktif referansları listeler.</summary>
public sealed class GetSYYClassificationDefOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYClassificationDefOptionsQuery,SYYClassificationDefOptions>
{
 public async Task<SYYClassificationDefOptions> Handle(GetSYYClassificationDefOptionsQuery r,CancellationToken ct)
 {
 var items0=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedSYYDiagnosisDefId))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
var items1=await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedForcesCommandDef))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
var items2=await db.RankGroupDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedRankGroup))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
var items3=await db.MilitaryClassGroupDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedMilitaryClassGroup))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
var items4=await db.HcConnectedDecisionDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedHcDecision))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
 return new SYYClassificationDefOptions(items0,items1,items2,items3,items4);
 }
}
