using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Defs.Responses;
using KLMN.Application.ICD10Defs.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Defs.Queries.GetICD10DefById;
public sealed class GetICD10DefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetICD10DefByIdQuery,ICD10DefResponse>
{
 public async Task<ICD10DefResponse> Handle(GetICD10DefByIdQuery r,CancellationToken ct)=>
 await db.ICD10Defs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(x=>
 new ICD10DefResponse(x.Id,x.Code,x.Name,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new ICD10DefNotFoundException();
}
