using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisOptions;
/// <summary>Sadece aktif seçenekler; düzenlemede daha önce seçilmiş pasif kayıtlar gösterilir.</summary>
public sealed class GetSYYDiagnosisOptionsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisOptionsQuery,SYYDiagnosisOptions>
{
 public async Task<SYYDiagnosisOptions> Handle(GetSYYDiagnosisOptionsQuery r,CancellationToken ct)
 {
 var types=await db.SYYDiagnosisTypeDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Code==r.CurrentType))
 .OrderBy(x=>x.Code).Select(x=>new SYYOption<int>(x.Code,x.Name,x.IsActive)).ToListAsync(ct);
 var slices=await db.SYYDiagnosisSliceDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Code==r.CurrentSlice))
 .OrderBy(x=>x.Code).Select(x=>new SYYOption<int>(x.Code,x.Name,x.IsActive)).ToListAsync(ct);
 var parents=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&x.Id!=r.CurrentId&&(x.IsActive||x.Id==r.CurrentParent))
 .OrderBy(x=>x.Name).Select(x=>new SYYOption<Guid>(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
 return new SYYDiagnosisOptions(types,slices,parents);
 }
}
