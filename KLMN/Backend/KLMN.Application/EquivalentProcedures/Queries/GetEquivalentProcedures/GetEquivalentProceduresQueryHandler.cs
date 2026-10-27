using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedures;
/// <summary>Her iki uçta arama, DTO projection ve server-side sayfalama.</summary>
public sealed class GetEquivalentProceduresQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetEquivalentProceduresQuery,PagedResult<EquivalentProcedureResponse>>
{
 public async Task<PagedResult<EquivalentProcedureResponse>> Handle(GetEquivalentProceduresQuery r,CancellationToken ct)
 {
  var q=db.EquivalentProcedureDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  if(r.ProcedureId.HasValue)q=q.Where(x=>x.ProcedureDefId==r.ProcedureId.Value||
                                         x.EquivalentProcedureId==r.ProcedureId.Value);
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  if(!string.IsNullOrWhiteSpace(r.Search)){
    var s=r.Search.Trim().ToUpperInvariant();
    q=q.Where(x=>x.Procedure.ProcedureCode.ToUpper().Contains(s)||
     x.Procedure.Name.ToUpper().Contains(s)||
     x.EquivalentProcedure.ProcedureCode.ToUpper().Contains(s)||
     x.EquivalentProcedure.Name.ToUpper().Contains(s));
  }
  var total=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.Procedure.Name).ThenBy(x=>x.EquivalentProcedure.Name).ThenBy(x=>x.Id)
   .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
   .Select(x=>new EquivalentProcedureResponse(x.Id,x.ProcedureDefId,
     x.Procedure.ProcedureCode,x.Procedure.Name,x.EquivalentProcedureId,
     x.EquivalentProcedure.ProcedureCode,x.EquivalentProcedure.Name,
     x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
  return new PagedResult<EquivalentProcedureResponse>{Items=items,TotalCount=total,
    PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
