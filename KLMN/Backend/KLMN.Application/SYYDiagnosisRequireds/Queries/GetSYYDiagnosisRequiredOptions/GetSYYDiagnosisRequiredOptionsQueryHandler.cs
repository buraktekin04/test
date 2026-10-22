using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequiredOptions;
public sealed class GetSYYDiagnosisRequiredOptionsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisRequiredOptionsQuery,IReadOnlyList<RequiredDiagnosisOption>>
{
 public async Task<IReadOnlyList<RequiredDiagnosisOption>> Handle(GetSYYDiagnosisRequiredOptionsQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
  .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedDiagnosisId))
  .OrderBy(x=>x.Name)
  .Select(x=>new RequiredDiagnosisOption(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
}
