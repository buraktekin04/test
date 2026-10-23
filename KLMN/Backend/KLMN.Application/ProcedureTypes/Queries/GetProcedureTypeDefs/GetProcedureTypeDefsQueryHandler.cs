using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ProcedureTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefs;
/// <summary>Tetkik türlerini kod sıralamasında sayfalar.</summary>
public sealed class GetProcedureTypeDefsQueryHandler(IKLMNDbContext db):
IRequestHandler<GetProcedureTypeDefsQuery,PagedResult<ProcedureTypeDefResponse>>
{
    public async Task<PagedResult<ProcedureTypeDefResponse>> Handle(GetProcedureTypeDefsQuery r,CancellationToken ct)
    {
        var q=db.ProcedureTypeDefs.IgnoreQueryFilters().AsNoTracking()
          .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
        if(!string.IsNullOrWhiteSpace(r.Search))
        {
           var search=r.Search.Trim().ToUpperInvariant();
           q=q.Where(x=>x.Name.ToUpper().Contains(search)||x.Code.ToString()==search);
        }
        if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
        var total=await q.CountAsync(ct);
        var items=await q.OrderBy(x=>x.Code).Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
           .Select(x=>new ProcedureTypeDefResponse(x.Id,x.Code,x.Name,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
        return new PagedResult<ProcedureTypeDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
    }
}
