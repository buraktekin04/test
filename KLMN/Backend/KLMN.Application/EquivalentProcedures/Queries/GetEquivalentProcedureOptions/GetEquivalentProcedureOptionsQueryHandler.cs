using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureOptions;
/// <summary>Dropdown seçenekleri SQL'de aranır; bütün tetkik tablosu yüklenmez.</summary>
public sealed class GetEquivalentProcedureOptionsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetEquivalentProcedureOptionsQuery,IReadOnlyList<EquivalentProcedureOption>>
{
 public async Task<IReadOnlyList<EquivalentProcedureOption>> Handle(GetEquivalentProcedureOptionsQuery r,CancellationToken ct)
 {
  var q=db.ProcedureDefs.IgnoreQueryFilters().AsNoTracking()
    .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedId));
  if(!string.IsNullOrWhiteSpace(r.Search)){
    var s=r.Search.Trim().ToUpperInvariant();
    q=q.Where(x=>x.ProcedureCode.ToUpper().Contains(s)||x.Name.ToUpper().Contains(s));
  }
  return await q.OrderByDescending(x=>x.Id==r.SelectedId).ThenBy(x=>x.ProcedureCode).ThenBy(x=>x.Name)
   .Take(60).Select(x=>new EquivalentProcedureOption(x.Id,x.ProcedureCode,
      x.Name,x.IsActive,x.ProcedureCode+" — "+x.Name)).ToListAsync(ct);
 }
}
