using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Diagnostics.Responses;
using KLMN.Application.ICD10Diagnostics.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticById;
public sealed class GetICD10DiagnosticByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetICD10DiagnosticByIdQuery,ICD10DiagnosticResponse>
{
 public async Task<ICD10DiagnosticResponse> Handle(GetICD10DiagnosticByIdQuery r,CancellationToken ct)=>
 await db.ICD10Diagnostics.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(ICD10DiagnosticResponse.Projection).SingleOrDefaultAsync(ct)??throw new ICD10DiagnosticNotFoundException();
}
