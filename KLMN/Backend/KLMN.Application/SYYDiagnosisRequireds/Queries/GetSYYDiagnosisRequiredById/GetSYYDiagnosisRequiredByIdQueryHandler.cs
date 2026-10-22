using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisRequireds.Exceptions;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequiredById;
public sealed class GetSYYDiagnosisRequiredByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisRequiredByIdQuery,SYYDiagnosisRequiredResponse>
{
 public async Task<SYYDiagnosisRequiredResponse> Handle(GetSYYDiagnosisRequiredByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisRequireds.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>x.Id==r.Id&&!x.IsDeleted).Select(x=>new SYYDiagnosisRequiredResponse(x.Id,x.SYYDiagnosisDefId,x.Diagnosis.Name,
 x.MustAll,x.MustOneOf,x.IsActive,x.Version,
 x.Details.Where(d=>!d.IsDeleted).OrderBy(d=>d.SortOrder)
   .Select(d=>new RequiredDetailResponse(d.Id,d.Description,d.SortOrder)).ToList()))
 .SingleOrDefaultAsync(ct)??throw new SYYDiagnosisRequiredNotFoundException();
}
